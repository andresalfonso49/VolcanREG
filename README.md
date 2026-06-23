# VolcanREG

VolcanREG es una PWA Blazor WebAssembly .NET 10 para registrar cargues de arena desde celulares Android, funcionar sin internet y sincronizar con Firebase Firestore cuando vuelve la conexion.

## Tecnologia

- .NET 10 Blazor WebAssembly PWA.
- Firebase Authentication con email y contrasena.
- Firebase Firestore plan Spark.
- IndexedDB para persistencia local offline-first.
- GitHub Pages para hosting.
- Chart.js para graficos.
- SheetJS Community desde navegador para exportar `.xlsx`.

## Configurar Firebase

1. Crea un proyecto en Firebase Console usando el plan gratuito Spark.
2. En Authentication, habilita `Email/password`.
3. En Firestore Database, crea la base en modo produccion.
4. Copia `firestore.rules` en la seccion Rules de Firestore y publica.
5. En Project settings, crea una Web App y copia la configuracion.
6. Pega esos valores en `wwwroot/appsettings.json`.

Ejemplo:

```json
{
  "Firebase": {
    "ApiKey": "TU_API_KEY",
    "AuthDomain": "tu-proyecto.firebaseapp.com",
    "ProjectId": "tu-proyecto",
    "StorageBucket": "tu-proyecto.appspot.com",
    "MessagingSenderId": "000000000000",
    "AppId": "1:000000000000:web:xxxxxxxxxxxxxxxxxxxxxx"
  }
}
```

## Usuarios

Crea estos usuarios en Firebase Authentication:

- `operador1@volcanreg.com`
- `operador2@volcanreg.com`
- `operador3@volcanreg.com`
- `admin@volcanreg.com`

Despues crea documentos en Firestore, coleccion `users`, usando como id el `uid` real de cada usuario. Usa `sample-data/users.firestore.json` como guia y cambia los `UID_*`.

Roles validos:

- `Operator`
- `Admin`

## Colecciones Firestore

- `users`
- `loadRecords`
- `drivers`
- `vehicles`
- `editLogs`
- `validationLogs`
- `syncLogs`

No hace falta crearlas manualmente; Firestore las crea al primer documento. Para pruebas puedes usar los archivos de `sample-data`.

## Ejecutar localmente

```bash
dotnet restore
dotnet build
dotnet run
```

Abre la URL local que muestre la consola.

## Probar offline

1. Inicia sesion como operador.
2. Abre DevTools del navegador y activa modo offline, o apaga datos/WiFi en Android.
3. Crea un cargue.
4. Verifica que queda en `Pendientes`.
5. Vuelve online.
6. Pulsa `Sincronizar ahora` o espera la sincronizacion automatica.

## Probar validacion administrativa

1. Inicia sesion como `admin@volcanreg.com`.
2. Entra a `Validacion`.
3. Revisa pendientes.
4. Marca registros como `Validado` o `Incorrecto`.
5. Para `Incorrecto`, registra motivo obligatorio.
6. Consulta el detalle para ver historial.

## Exportar Excel

En `Reportes`, ajusta filtros y pulsa `Exportar Excel`. El archivo incluye hojas para:

- Reporte final.
- Pendientes de revision.
- Incorrectos.
- Historico completo.
- Resumen.
- Por operador.
- Por placa.
- Por conductor.
- Por dia de semana.

## Publicar en GitHub Pages

Publica la salida de:

```bash
dotnet publish -c Release
```

La carpeta resultante queda bajo `bin/Release/net10.0/publish/wwwroot`. Para repositorios publicados bajo subcarpeta, ajusta el `<base href="/">` de `wwwroot/index.html` al nombre del repositorio, por ejemplo `/VolcanREG/`.

## Instalar en Android

1. Abre la URL publicada en Chrome Android.
2. Usa `Agregar a pantalla principal` o `Instalar app`.
3. Entra una vez con internet para cargar la app y sesion.
4. Luego puedes registrar cargues sin conexion.

## Nota de seguridad

Sin backend propio ni Cloud Functions, Firestore Rules cubre roles, lectura/escritura basica y bloqueo de borrado fisico. Validaciones complejas como auditoria avanzada campo por campo se implementan en cliente en esta version. Para una version futura con mayor criticidad, conviene agregar un backend minimo o Cloud Functions.
