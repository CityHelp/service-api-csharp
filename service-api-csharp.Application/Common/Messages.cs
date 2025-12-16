using service_api_csharp.Application.DTOs;

namespace service_api_csharp.Application.Common;

public static class Messages
{ 
    public static class Cloudinary
    {
        public const string ImageUploadSuccessfully = "Image uploaded successfully";
        public const string NoFileProvided = "No file provided";
        public const string FileTypeNotAllowed = "File type not allowed";
        public const string FileTooBig = "File too large";
    }

    public static class Coordinates
    {
        public const string LatitudeRequired = "Latitude is required";
        public const string LongitudeRequired = "Longitude is required";
        public const string LatitudeIsANumber = "Latitude must be a valid number";
        public const string LongitudeIsANumber = "Longitude must be a valid number";
        public const string LatitudeIsAValidNumber = "Latitude must be between -90 and 90";
        public const string LongitudeIsAValidNumber = "Longitude must be between -180 and 180";
        public const string GeneralInvalidCoordinates = "Invalid coordinates";
    }

    public static class Errors
    {
        public const string GenericField = "Invalid fields";
        public const string UnexpectedError = "An unexpected error occurred";
        public const string Unauthorized = "Unauthorized";
    }

    public static class SystemDirectory
    {
        public const string FoundDirectories = "Emergency sites near the location";
    }

    public static class ReportsCategories
    {
        public const string CategoryNotFound = "Category not found";
    }

    public static class Reports
    {
        public const string ReportsFound = "Reports found";
        public const string ReportRegistered = "Report created successfully";
        public const string ReportDeleted = "Report deleted successfully";
        public const string ReportUpdated = "Report updated successfully";
        public const string ReportNotFound = "Report not found";
        public const string AlreadyRequestedDeletion = "User already requested deletion";
        public const string RegisteredDeleteRequest = "Deletion request registered successfully";
    }
}
