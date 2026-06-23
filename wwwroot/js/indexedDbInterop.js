(function () {
  const dbName = "VolcanREG";
  const dbVersion = 1;
  let dbPromise;

  function openDb() {
    if (dbPromise) {
      return dbPromise;
    }

    dbPromise = new Promise((resolve, reject) => {
      const request = indexedDB.open(dbName, dbVersion);

      request.onupgradeneeded = () => {
        const db = request.result;
        if (!db.objectStoreNames.contains("loadRecords")) {
          db.createObjectStore("loadRecords", { keyPath: "localId" });
        }
        if (!db.objectStoreNames.contains("drivers")) {
          db.createObjectStore("drivers", { keyPath: "driverCc" });
        }
        if (!db.objectStoreNames.contains("vehicles")) {
          db.createObjectStore("vehicles", { keyPath: "vehiclePlate" });
        }
        if (!db.objectStoreNames.contains("settings")) {
          db.createObjectStore("settings", { keyPath: "key" });
        }
      };

      request.onsuccess = () => resolve(request.result);
      request.onerror = () => reject(request.error);
    });

    return dbPromise;
  }

  async function tx(storeName, mode, action) {
    const db = await openDb();
    return new Promise((resolve, reject) => {
      const transaction = db.transaction(storeName, mode);
      const store = transaction.objectStore(storeName);
      const result = action(store);
      transaction.oncomplete = () => resolve(result);
      transaction.onerror = () => reject(transaction.error);
    });
  }

  async function getAll(storeName) {
    const db = await openDb();
    return new Promise((resolve, reject) => {
      const transaction = db.transaction(storeName, "readonly");
      const request = transaction.objectStore(storeName).getAll();
      request.onsuccess = () => resolve(request.result ?? []);
      request.onerror = () => reject(request.error);
    });
  }

  async function getSetting(key) {
    const db = await openDb();
    return new Promise((resolve, reject) => {
      const transaction = db.transaction("settings", "readonly");
      const request = transaction.objectStore("settings").get(key);
      request.onsuccess = () => resolve(request.result?.value ?? null);
      request.onerror = () => reject(request.error);
    });
  }

  async function setSetting(key, value) {
    await tx("settings", "readwrite", store => store.put({ key, value }));
  }

  async function getRecord(localId) {
    const db = await openDb();
    return new Promise((resolve, reject) => {
      const transaction = db.transaction("loadRecords", "readonly");
      const request = transaction.objectStore("loadRecords").get(localId);
      request.onsuccess = () => resolve(request.result ?? null);
      request.onerror = () => reject(request.error);
    });
  }

  window.volcanIndexedDb = {
    initialize: () => openDb(),

    getDeviceId: async () => {
      let deviceId = await getSetting("deviceId");
      if (!deviceId) {
        deviceId = crypto.randomUUID();
        await setSetting("deviceId", deviceId);
      }
      return deviceId;
    },

    saveLoadRecord: async (record) => {
      await tx("loadRecords", "readwrite", store => store.put(record));
    },

    getAllLoadRecords: () => getAll("loadRecords"),

    markSynced: async (localId, serverRecordId) => {
      const record = await getRecord(localId);
      if (!record) {
        return;
      }

      record.syncStatus = "Synced";
      record.serverRecordId = serverRecordId;
      record.syncedAtUtc = new Date().toISOString();
      record.lastSyncError = null;
      await tx("loadRecords", "readwrite", store => store.put(record));
    },

    markSyncStatus: async (localId, syncStatus, error) => {
      const record = await getRecord(localId);
      if (!record) {
        return;
      }

      record.syncStatus = syncStatus;
      record.lastSyncError = error;
      await tx("loadRecords", "readwrite", store => store.put(record));
    },

    saveRecentDriverAndVehicle: async (record) => {
      const now = new Date().toISOString();
      await tx("drivers", "readwrite", store => store.put({
        driverCc: record.driverCc,
        driverName: record.driverName,
        lastVehiclePlate: record.vehiclePlate,
        createdAtUtc: now,
        updatedAtUtc: now
      }));
      await tx("vehicles", "readwrite", store => store.put({
        vehiclePlate: record.vehiclePlate,
        lastDriverCc: record.driverCc,
        lastDriverName: record.driverName,
        createdAtUtc: now,
        updatedAtUtc: now
      }));
    },

    getDrivers: () => getAll("drivers"),
    getVehicles: () => getAll("vehicles"),
    getLastSyncUtc: () => getSetting("lastSyncUtc"),
    setLastSyncUtc: (value) => setSetting("lastSyncUtc", value),
    getCachedUserProfile: () => getSetting("cachedUserProfile"),
    setCachedUserProfile: (profile) => setSetting("cachedUserProfile", profile),
    clearCachedUserProfile: async () => {
      await setSetting("cachedUserProfile", null);
      await setSetting("lastProfileCheckUtc", null);
    },
    getLastProfileCheckUtc: () => getSetting("lastProfileCheckUtc"),
    setLastProfileCheckUtc: (value) => setSetting("lastProfileCheckUtc", value)
  };
})();
