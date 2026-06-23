namespace VolcanREG.Security;

public static class SyncStatuses
{
    public const string Pending = "Pending";
    public const string Syncing = "Syncing";
    public const string Synced = "Synced";
    public const string Error = "Error";
}

public static class ValidationStatuses
{
    public const string NotValidated = "NotValidated";
    public const string Validated = "Validated";
    public const string Incorrect = "Incorrect";

    public static readonly string[] All =
    [
        NotValidated,
        Validated,
        Incorrect
    ];
}

public static class IncorrectReasons
{
    public static readonly string[] Suggested =
    [
        "Devolucion de material",
        "Placa mal registrada",
        "Cedula incorrecta",
        "Volumen mal digitado",
        "Cargue duplicado",
        "Cargue no realizado",
        "Cliente incorrecto",
        "Otro"
    ];
}
