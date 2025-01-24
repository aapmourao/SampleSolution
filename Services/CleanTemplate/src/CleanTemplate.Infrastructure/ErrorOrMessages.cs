using ErrorOr;

namespace UserManagement.Infrastructure;
internal static class ErrorOrMessages
{
    internal static Error ValidationPasswordTooWeak => Error.Validation(
        code: "PasswordTooWeak",
        description: "Password too weak");
}
