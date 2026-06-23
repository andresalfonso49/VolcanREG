(function () {
  let firebaseApp;
  let auth;
  let db;
  let modulesPromise;
  let firebaseOptions;

  function ensureConfig(options) {
    if (!options?.apiKey || !options?.projectId) {
      throw new Error("Configura Firebase en wwwroot/appsettings.json antes de iniciar sesion.");
    }
  }

  async function modules() {
    if (!modulesPromise) {
      modulesPromise = Promise.all([
        import("https://www.gstatic.com/firebasejs/10.14.1/firebase-app.js"),
        import("https://www.gstatic.com/firebasejs/10.14.1/firebase-auth.js"),
        import("https://www.gstatic.com/firebasejs/10.14.1/firebase-firestore.js")
      ]).then(([app, authModule, firestore]) => ({ app, authModule, firestore }));
    }
    return modulesPromise;
  }

  function fromFirestore(data) {
    const result = {};
    for (const [key, value] of Object.entries(data ?? {})) {
      result[key] = value?.toDate ? value.toDate().toISOString() : value;
    }
    return result;
  }

  function clean(value) {
    const output = {};
    for (const [key, current] of Object.entries(value ?? {})) {
      if (current !== undefined) {
        output[key] = current;
      }
    }
    return output;
  }

  function firestoreRecord(record) {
    return clean({
      clientRecordId: record.clientRecordId,
      loadedAtLocal: record.loadedAtLocal,
      loadedAtUtc: record.loadedAtUtc,
      operatorId: record.operatorId,
      operatorNameSnapshot: record.operatorNameSnapshot,
      material: record.material,
      volumeM3: record.volumeM3,
      vehiclePlate: record.vehiclePlate,
      driverName: record.driverName,
      driverCc: record.driverCc,
      customerName: record.customerName,
      latitude: record.latitude,
      longitude: record.longitude,
      observations: record.observations,
      validationStatus: record.validationStatus ?? "NotValidated",
      validatedByUserId: record.validatedByUserId,
      validatedByUserName: record.validatedByUserName,
      validatedAtUtc: record.validatedAtUtc,
      validationNotes: record.validationNotes,
      incorrectReason: record.incorrectReason,
      deviceId: record.deviceId,
      createdAtUtc: record.createdAtUtc,
      updatedAtUtc: record.updatedAtUtc,
      syncedAtUtc: record.syncedAtUtc
    });
  }

  async function collectionDocs(collectionName) {
    const { firestore } = await modules();
    const snap = await firestore.getDocs(firestore.collection(db, collectionName));
    return snap.docs.map(doc => ({ ...fromFirestore(doc.data()), serverRecordId: doc.id, id: doc.id }));
  }

  window.volcanFirebase = {
    initialize: async (options) => {
      if (firebaseApp) {
        return;
      }

      ensureConfig(options);
      firebaseOptions = options;
      const { app, authModule, firestore } = await modules();
      firebaseApp = app.initializeApp(options);
      auth = authModule.getAuth(firebaseApp);
      db = firestore.getFirestore(firebaseApp);
    },

    signIn: async (email, password) => {
      const { authModule } = await modules();
      const credential = await authModule.signInWithEmailAndPassword(auth, email, password);
      return {
        uid: credential.user.uid,
        email: credential.user.email,
        displayName: credential.user.displayName,
        idToken: await credential.user.getIdToken()
      };
    },

    signOut: async () => {
      const { authModule } = await modules();
      await authModule.signOut(auth);
    },

    getCurrentUser: async () => {
      const { authModule } = await modules();
      return await new Promise((resolve) => {
        const unsubscribe = authModule.onAuthStateChanged(auth, async user => {
          unsubscribe();
          resolve(user ? {
            uid: user.uid,
            email: user.email,
            displayName: user.displayName,
            idToken: await user.getIdToken()
          } : null);
        });
      });
    },

    getUserProfile: async (uid) => {
      const { firestore } = await modules();
      const ref = firestore.doc(db, "users", uid);
      const snap = await firestore.getDoc(ref);
      return snap.exists() ? fromFirestore(snap.data()) : null;
    },

    getUsers: () => collectionDocs("users"),
    getDrivers: () => collectionDocs("drivers"),
    getVehicles: () => collectionDocs("vehicles"),

    createUser: async (email, password, displayName, role, isActive) => {
      const { app, authModule, firestore } = await modules();
      const secondaryName = `user-create-${Date.now()}`;
      const secondaryApp = app.initializeApp(firebaseOptions, secondaryName);
      const secondaryAuth = authModule.getAuth(secondaryApp);

      try {
        const credential = await authModule.createUserWithEmailAndPassword(secondaryAuth, email, password);
        await authModule.updateProfile(credential.user, { displayName });
        await authModule.signOut(secondaryAuth);

        const now = new Date().toISOString();
        const profile = {
          uid: credential.user.uid,
          displayName,
          email,
          role,
          isActive,
          createdAtUtc: now,
          updatedAtUtc: now
        };

        await firestore.setDoc(firestore.doc(db, "users", credential.user.uid), clean(profile));
        return profile;
      } finally {
        await app.deleteApp(secondaryApp);
      }
    },

    updateUserProfile: async (profile) => {
      const { firestore } = await modules();
      const updated = clean({
        uid: profile.uid,
        displayName: profile.displayName,
        email: profile.email,
        role: profile.role,
        isActive: profile.isActive,
        createdAtUtc: profile.createdAtUtc,
        updatedAtUtc: new Date().toISOString()
      });
      await firestore.setDoc(firestore.doc(db, "users", profile.uid), updated, { merge: true });
    },

    findLoadRecordByClientRecordId: async (clientRecordId) => {
      const { firestore } = await modules();
      const ref = firestore.doc(db, "loadRecords", clientRecordId);
      const snap = await firestore.getDoc(ref);
      return snap.exists() ? { ...fromFirestore(snap.data()), serverRecordId: snap.id } : null;
    },

    createLoadRecord: async (record) => {
      const { firestore } = await modules();
      const documentId = record.clientRecordId;
      await firestore.setDoc(firestore.doc(db, "loadRecords", documentId), firestoreRecord(record));
      return documentId;
    },

    getLoadRecords: () => collectionDocs("loadRecords"),

    getLoadRecord: async (id) => {
      const { firestore } = await modules();
      const snap = await firestore.getDoc(firestore.doc(db, "loadRecords", id));
      return snap.exists() ? { ...fromFirestore(snap.data()), serverRecordId: snap.id } : null;
    },

    updateLoadRecord: async (record) => {
      const { firestore } = await modules();
      await firestore.updateDoc(firestore.doc(db, "loadRecords", record.serverRecordId ?? record.clientRecordId), firestoreRecord(record));
    },

    createEditLogs: async (logs) => {
      const { firestore } = await modules();
      for (const log of logs) {
        await firestore.setDoc(firestore.doc(db, "editLogs", log.id), clean(log));
      }
    },

    getEditLogs: async (loadRecordId) => {
      const { firestore } = await modules();
      const q = firestore.query(firestore.collection(db, "editLogs"), firestore.where("loadRecordId", "==", loadRecordId));
      const snap = await firestore.getDocs(q);
      return snap.docs.map(doc => ({ ...fromFirestore(doc.data()), id: doc.id }));
    },

    createValidationLog: async (log) => {
      const { firestore } = await modules();
      await firestore.setDoc(firestore.doc(db, "validationLogs", log.id), clean(log));
    },

    getValidationLogs: async (loadRecordId) => {
      const { firestore } = await modules();
      const q = firestore.query(firestore.collection(db, "validationLogs"), firestore.where("loadRecordId", "==", loadRecordId));
      const snap = await firestore.getDocs(q);
      return snap.docs.map(doc => ({ ...fromFirestore(doc.data()), id: doc.id }));
    },

    upsertDriverAndVehicle: async (record) => {
      const { firestore } = await modules();
      const now = new Date().toISOString();
      await firestore.setDoc(firestore.doc(db, "drivers", record.driverCc), clean({
        driverCc: record.driverCc,
        driverName: record.driverName,
        lastVehiclePlate: record.vehiclePlate,
        updatedAtUtc: now
      }), { merge: true });
      await firestore.setDoc(firestore.doc(db, "vehicles", record.vehiclePlate), clean({
        vehiclePlate: record.vehiclePlate,
        lastDriverCc: record.driverCc,
        lastDriverName: record.driverName,
        updatedAtUtc: now
      }), { merge: true });
    },

    createSyncLog: async (log) => {
      const { firestore } = await modules();
      await firestore.addDoc(firestore.collection(db, "syncLogs"), clean(log));
    },

    resetOperationalDatabase: async () => {
      const { firestore } = await modules();
      const collections = [
        "loadRecords",
        "drivers",
        "vehicles",
        "editLogs",
        "validationLogs",
        "syncLogs"
      ];

      for (const collectionName of collections) {
        let snapshot = await firestore.getDocs(firestore.collection(db, collectionName));
        while (!snapshot.empty) {
          const batch = firestore.writeBatch(db);
          snapshot.docs.slice(0, 450).forEach(doc => batch.delete(doc.ref));
          await batch.commit();

          if (snapshot.docs.length <= 450) {
            break;
          }

          snapshot = await firestore.getDocs(firestore.collection(db, collectionName));
        }
      }
    }
  };
})();
