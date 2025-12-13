using service_api_csharp.Application.DTOs;

namespace service_api_csharp.Application.Common;

public static class Messages
{
    public static class Jwt
    {
        public const string ExpiredToken = "Expired token";
    }

    public static class Clodinary
    {
        public const string ImageUploadSucesfully = "Image uploaded successfully";
        public const string NoFileProvided = "No file provided";
        public const string TypeFileNotAllowed = "File type not allowed";
        public const string FileTooBig = "File too large";
    }


    public static class Coordinates
    {
        public const string LatitudeObligatory = "Latitude is obligatory";
        public const string LongitudeObligatory = "Longitude is obligatory";
        public const string LatitudeBeANumber = "Latitude must be a valid number";
        public const string LongitudeBeANumber = "Longitude must be a valid number";
        public const string LatitudeBeAValidNumber = "Latitude must be between -90 and 90";
        public const string LongitudeBeAValidNumber = "Longitude must be between -180 and 180";
        public const string GeneralInvalidCoordinates = "Invalid coordinates";
    }

    public static class Errors
    {
        public const string GenericField = "Invalid fields";
        public const string UnexpectedError = "An unexpected error occured";
        public const string InvalidCredentials = "Invalid credentials";
        public const string InvalidCredentialsError = "Email o password not found";
        public const string Unauthorized = "Unauthorized";
        public const string InvalidToken = "Invalid token";
    }

    public static class SystemDirectory
    {
        public const string FoundDirectories = "Directories near of the person";
    }
}
