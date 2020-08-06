# Integración con docuware
Documentación y ejemplos de integración
### Enlaces importantes:
- [URL Integration](https://help.docuware.com/#/home/80582/2/2)
- API
  - [.Net API](https://developer.docuware.com/dotNet/66b2ed1e-2aef-452a-97cd-5014bbf0242b.html)
  - [REST API](https://developer.docuware.com/rest/index.html)
- Servicios de extensión:
  - [Workflow service](https://developer.docuware.com/Extension_Services/extension_services.html)
  - [Validation service](https://developer.docuware.com/Extension_Services/examples/DocuWorld_2019_Integration_Workshop.html)
## Configuración del entorno
  - [Visual Studio Code](https://code.visualstudio.com/)
  - [.Net Core 3.1 SDK](https://dotnet.microsoft.com/download/dotnet-core/3.1)
  - Acceso al API de DocuWare (https://{host}/**Docuware/Platform**)
  - Credenciales de DocuWare con permiso de busqueda y almacenamiento
  - Guid del Archivador
## Ejemplos:
***Nota: Antes de ejecutar la aplicación debe actualizar el archivo "appsettings.json" de cada proyecto con la información correcta***
#### DocuWareApi (VB)
  Ejemplo de utilizando la Api de Docuware, Consulta y Carga de documentos
#### DocuWareRest (C#)
  Ejemplo de utilizando la API REST, con el Objeto HttpClient de .Net Core
