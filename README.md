# Fakestagram

This is a simple API based on .NET 6. It imitates the backend of a social media, such as Instagram, thus the name - 'Fakestagram'.

The application has the following features:

- File upload and persistence, with file type verification in place.
- User authentication and authorization via Bearer JWT scheme. User hierarchy with 2 types of users: `Regular` user and an `Administrator`. The administrator can delete and edit entities, which he doesn't own.
- CRUD posts, comments, user profile. Follow users and like their posts and comments.
- Embedded Swagger API documentation within the application.