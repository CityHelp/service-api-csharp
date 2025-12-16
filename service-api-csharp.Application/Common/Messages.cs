using service_api_csharp.Application.DTOs;

namespace service_api_csharp.Application.Common;

public static class Messages
{ 
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
        public const string Unauthorized = "Unauthorized";
    }

    public static class SystemDirectory
    {
        public const string FoundDirectories = "Directories near of the person";
    }

    public static class ReportsCategories
    {
        public const string CategorNotFound = "Category not found";
    }

    public static class Reports
    {
        public const string ReportsFound = "Reports found";
        public const string ReportRegistered = "Report created successfully";
        public const string ReportDeleted = "Report deleted successfully";
        public const string ReportUpdated = "Report updated successfully";
        public const string ReportNotFound = "Report not found";
        public const string AlreadyRequestDeletion = "User already request deletion";
        public const string RegisteredRequestDeletion = "Registered requeset deletion";
    }
}
