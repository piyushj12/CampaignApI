# README - Campaign Management API

## Overview
The Campaign Management API is a comprehensive .NET Web API solution tailored for managing marketing campaigns. This API facilitates operations such as adding, updating, deleting, and retrieving campaign data. A significant aspect of this API is its utilization of MongoDB for database management, offering scalable and flexible storage solutions. Moreover, the API is designed with clarity and maintainability in mind, incorporating custom data annotations for effective validation.

## Solution Architecture
The architecture of this solution is meticulously organized into distinct projects for Data (entity), Model, Service, and API. This division ensures a streamlined separation of concerns, enhancing the overall structure and readability of the codebase.

## Key Features:
- **MongoDB Database Integration**: All data is stored in MongoDB, providing robust and scalable data handling.
- **Data Validation**: Utilizes both data annotation and custom data annotation for rigorous data validation.
- **Model-Based Responses**: API endpoints return Model objects, not direct entity objects, for improved data encapsulation.
- **Auto Mapper**: This feature is employed for efficient mapping between model and entity, simplifying data transfer processes.
- **Error Handling and Logging**: The API is equipped with comprehensive error handling and logging mechanisms for reliable and efficient debugging.


## API Endpoints

### POST: Add Campaign
- **Endpoint**: `POST /api/campaigns`
- **Description**: Adds a new campaign.
- **Body**: Comprises all the requisite fields of a campaign.
- **Validations**: Ensures the presence of the name, at least one state, and a minimum of two days of the week. The end date is mandated to be later than the start date. It also validates state and day values against predefined lists and prevents duplicate campaign names.

### PUT: Update Campaign
- **Endpoint**: `PUT /api/campaigns/{campaignId}`
- **Description**: Updates an existing campaign.
- **Query Parameters**: Requires a non-empty GUID as `campaignId`.
- **Validations**: Similar required fields as in the Add Campaign endpoint, with an additional check to ensure the campaign's existence before update.

### DELETE: Delete Campaign
- **Endpoint**: `DELETE /api/campaigns/{campaignId}`
- **Description**: Deletes a specified campaign.
- **Query Parameters**: `campaignId` as a non-empty GUID.
- **Validations**: The campaign must be inactive and existent to qualify for deletion.

### GET: Get Campaign by ID
- **Endpoint**: `GET /api/campaigns/{campaignId}`
- **Description**: Retrieves a campaign using its unique ID.
- **Query Parameters**: `campaignId` as a non-empty GUID.

### GET: Get Campaigns
- **Endpoint**: `GET /api/campaigns`
- **Description**: Fetches a list of campaigns based on various query parameters.
- **Query Parameters**: Includes pageIndex, pageSize, searchName, searchState, searchDay, sortField, and sortDirection.

## Setup and Installation
1. **Clone the Repository**: Acquire the codebase by cloning the repository.
2. **Open in Visual Studio**: Launch the solution using Visual Studio.
3. **Restore NuGet Packages**: Ensure all required NuGet packages are properly restored.
4. **Configure MongoDB**: Set up the MongoDB connection string in the `appsettings.json` file.
5. **Run the Application**: Initiate the application to start using the API.

## Dependencies
- .NET 5 or a more recent version.
- MongoDB Driver for .NET for database connectivity.
- Entity Framework Core (if utilized).
- AutoMapper for object mapping.

## Contributing
We openly welcome contributions to this project. Adherence to the project's coding standards is appreciated. Kindly submit your pull requests for review.

## Contact
For queries or suggestions, feel free to reach out at piyushnjain23@gmail.com.
