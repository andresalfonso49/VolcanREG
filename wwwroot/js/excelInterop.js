(function () {
  async function sheetJs() {
    return import("https://cdn.sheetjs.com/xlsx-0.20.3/package/xlsx.mjs");
  }

  function rows(records) {
    return records.map(x => ({
      FechaLocal: x.loadedAtLocal,
      FechaUtc: x.loadedAtUtc,
      Operador: x.operatorNameSnapshot,
      Material: x.material,
      M3: x.volumeM3,
      Placa: x.vehiclePlate,
      Conductor: x.driverName,
      Cedula: x.driverCc,
      Cliente: x.customerName,
      EstadoValidacion: x.validationStatus,
      MotivoIncorrecto: x.incorrectReason,
      ValidadoPor: x.validatedByUserName,
      ValidadoEnUtc: x.validatedAtUtc,
      Observaciones: x.observations,
      ClientRecordId: x.clientRecordId
    }));
  }

  function summary(records) {
    const total = records.length;
    const validated = records.filter(x => x.validationStatus === "Validated");
    const pending = records.filter(x => x.validationStatus === "NotValidated");
    const incorrect = records.filter(x => x.validationStatus === "Incorrect");
    const sum = values => values.reduce((acc, x) => acc + Number(x.volumeM3 || 0), 0);
    return [
      { Metrica: "Total cargues", Valor: total },
      { Metrica: "Total m3", Valor: sum(records) },
      { Metrica: "Total validados", Valor: validated.length },
      { Metrica: "Total pendientes", Valor: pending.length },
      { Metrica: "Total incorrectos", Valor: incorrect.length },
      { Metrica: "Porcentaje incorrectos", Valor: total ? incorrect.length / total * 100 : 0 },
      { Metrica: "Promedio m3 validado", Valor: validated.length ? sum(validated) / validated.length : 0 }
    ];
  }

  function group(records, keySelector) {
    const map = new Map();
    for (const record of records) {
      const key = keySelector(record) || "Sin dato";
      const current = map.get(key) ?? { Llave: key, Cargues: 0, M3: 0 };
      current.Cargues += 1;
      current.M3 += Number(record.volumeM3 || 0);
      map.set(key, current);
    }
    return Array.from(map.values());
  }

  window.volcanExcel = {
    exportLoadRecords: async (records) => {
      const XLSX = await sheetJs();
      const list = Array.from(records ?? []);
      const wb = XLSX.utils.book_new();

      XLSX.utils.book_append_sheet(wb, XLSX.utils.json_to_sheet(rows(list.filter(x => x.validationStatus === "Validated"))), "Reporte final");
      XLSX.utils.book_append_sheet(wb, XLSX.utils.json_to_sheet(rows(list.filter(x => x.validationStatus === "NotValidated"))), "Pendientes revision");
      XLSX.utils.book_append_sheet(wb, XLSX.utils.json_to_sheet(rows(list.filter(x => x.validationStatus === "Incorrect"))), "Incorrectos");
      XLSX.utils.book_append_sheet(wb, XLSX.utils.json_to_sheet(rows(list)), "Historico completo");
      XLSX.utils.book_append_sheet(wb, XLSX.utils.json_to_sheet(summary(list)), "Resumen");
      XLSX.utils.book_append_sheet(wb, XLSX.utils.json_to_sheet(group(list, x => x.operatorNameSnapshot)), "Por operador");
      XLSX.utils.book_append_sheet(wb, XLSX.utils.json_to_sheet(group(list, x => x.vehiclePlate)), "Por placa");
      XLSX.utils.book_append_sheet(wb, XLSX.utils.json_to_sheet(group(list, x => x.driverName)), "Por conductor");
      XLSX.utils.book_append_sheet(wb, XLSX.utils.json_to_sheet(group(list, x => new Date(x.loadedAtLocal).toLocaleDateString("es-CO", { weekday: "long" }))), "Por dia semana");

      XLSX.writeFile(wb, `VolcanREG-${new Date().toISOString().slice(0, 10)}.xlsx`);
    }
  };
})();
