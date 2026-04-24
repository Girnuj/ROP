# 🚂 Railway Oriented Programming (ROP) en C#

Implementación del **Patrón Result** aplicando Railway Oriented Programming en C#, concepto original de [Scott Wlaschin](https://fsharpforfunandprofit.com/rop/).

---

## ¿Qué es Railway Oriented Programming?

ROP es un patrón que mezcla lo mejor de la **programación funcional** con la **programación orientada a objetos**, con el objetivo de gestionar errores de forma limpia y elegante, sin la necesidad de lanzar excepciones ni de anidar múltiples `if` de comprobación.

La idea central es que cada función del flujo tiene **dos posibles caminos de salida**: el camino feliz (*happy path*) y el camino de error (*failure path*). Una vez que se produce un error, el resto de funciones se omiten automáticamente y el error se propaga hasta el final.

### Camino feliz vs camino de error

Sin ROP, el control de errores típico se ve así:

```csharp
public string BasicAccountCreation(Account account)
{
    string accountValidated = ValidateNewAccount(account);
    if (!string.IsNullOrWhiteSpace(accountValidated))
        return accountValidated;

    bool isSaved = SaveUser(account);
    if (!isSaved)
        return "Error actualizando la base de datos";

    bool isSent = SendCode(account);
    if (!isSent)
        return "Error Enviando el email";

    return "Usuario añadido correctamente";
}
```

Con ROP, el mismo flujo queda así:

```csharp
public Result<UserAccount> AddUser(UserAccount userAccount)
{
    return ValidateUser(userAccount)
        .Bind(AddUserToDatabase)
        .Bind(SendEmail)
        .Map(_ => userAccount);
}
```

### Diagrama del flujo

![Flow completo POO vs ROP](https://www.netmentor.es/Imagen/d1c50527-8dbc-433a-ae6e-270e630468e1.jpg)

Cada función recibe el resultado anterior. Si todo ha ido bien, ejecuta su lógica. Si ya hay un error, lo propaga sin ejecutar nada:

![Explicación ROP](https://www.netmentor.es/Imagen/9dfc2f9f-c181-40f1-b611-7298b591514f.jpg)
---

## Implementación

### `Result<T>`

El núcleo del patrón es el tipo `Result<T>`, un `struct` que encapsula tanto el valor de éxito como los posibles errores:

```csharp
public struct Result<T>
{
    public readonly T Value;
    public readonly ImmutableArray<string> Errors;
    public bool Success => Errors.Length == 0;

    public Result(T value)
    {
        Value = value;
        Errors = ImmutableArray<string>.Empty;
    }

    public Result(ImmutableArray<string> errors)
    {
        Value = default(T);
        Errors = errors;
    }
}
```

### `Unit`

Para métodos que devuelven `void`, se usa el tipo `Unit` como sustituto compatible con `Result<T>`:

```csharp
public sealed class Unit
{
    public static readonly Unit Value = new Unit();
    private Unit() { }
}
```

### Métodos de extensión

Toda la lógica de encadenamiento se implementa como [extension methods](https://www.netmentor.es/Entrada/extension-methods):

| Método | Descripción |
|--------|-------------|
| `.Bind<T, U>()` | Ejecuta el siguiente paso si hay éxito; propaga el error si no |
| `.Map<T, U>()` | Transforma el valor de éxito de `T` a `U` |
| `.Then<T>()` | Ejecuta una acción sin transformar el resultado (para efectos secundarios) |

```csharp
// Bind - encadena funciones que devuelven Result<T>
public static Result<U> Bind<T, U>(this Result<T> r, Func<T, Result<U>> method)
{
    return r.Success
        ? method(r.Value)
        : Result.Failure<U>(r.Errors);
}

// Map - transforma el valor interno
public static Result<U> Map<T, U>(this Result<T> r, Func<T, U> mapper)
{
    return r.Success
        ? Result.Success(mapper(r.Value))
        : Result.Failure<U>(r.Errors);
}

// Then - efecto secundario, devuelve el mismo resultado
public static Result<T> Then<T>(this Result<T> r, Action<T> action)
{
    if (r.Success) action(r.Value);
    return r;
}
```

---

## Objetivos del patrón

- ✅ **Consolidar errores** — todos los errores se recogen en un único lugar al final del flujo.
- ✅ **Funciones pequeñas y enfocadas** — cada paso tiene una única responsabilidad.
- ✅ **Flujo de datos visible** — el encadenamiento de `.Bind()` expresa el flujo de la lógica de negocio de forma explícita.
- ✅ **Una función por caso de uso** — el método principal se convierte en una descripción del proceso.

---

## Versión asíncrona

El patrón es totalmente compatible con código asíncrono usando `Task<Result<T>>`:

```csharp
public static async Task<Result<U>> Bind<T, U>(
    this Task<Result<T>> result,
    Func<T, Task<Result<U>>> method)
{
    var r = await result;
    return r.Success
        ? await method(r.Value)
        : Result.Failure<U>(r.Errors);
}
```

---

## Referencias

- 📖 Mas info: [Patrón Result en C# — NetMentor](https://www.netmentor.es/entrada/railway-oriented-programming)
- 💡 Concepto original: [Railway Oriented Programming — Scott Wlaschin](https://fsharpforfunandprofit.com/rop/)
