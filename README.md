EventEase Web Application:

EventEase is an ASP.NET Core MVC web application designed to manage events, venues, and bookings efficiently. It is built using C#, Entity Framework Core, and SQL Server, the application provides a easy user interface for creating, editing, and managing event bookings in real time.

---
Table of Contents

- [Overview]
- [Features]
- [Technology Stack]
- [Setup Instructions]
- [Usage Guide]
- [Database Structure]
- [Known Issues]
- [Future Enhancements]

---
Overview

EventEase simplifies the process of booking venues for events by allowing users to:

- View all current events and venues
- Create new events and venues
- Book a venue for a specific event
- Edit or cancel bookings

The app ensures no double-bookings occur so that management of venues is seamless

---
Features

-  View a list of all bookings
-  Create new bookings with event and venue selection
-  Edit and delete existing bookings
-  Manage venue and event details
-  Automatically timestamps bookings using system date and time
-  Server-side validation with anti-forgery protection
-  Entity relationships maintained through Entity Framework Core
-  MVC architecture for separation of concerns

---
Technology Stack

- Frontend: Razor Pages, HTML5.
- Backend: ASP.NET Core MVC (.NET 6+)
- Database: SQL Server (with EF Core)
- ORM: Entity Framework Core
- IDE: Visual Studio 2022 or later

---
Setup Instructions
Prerequisites

- [.NET 6.0 SDK or later](https://dotnet.microsoft.com/download)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) with ASP.NET and web development tools
- [SQL Server LocalDB or full version](https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb)

---
Usage Guide

Bookings
Go to Booking

View a list of all current bookings

Click "Create" to add a new booking by selecting an event, venue, and booking date

Click "Edit" to modify an existing booking

Click "Delete" to remove a booking

Venues
Go to Venue

View, add, or edit venues, including name, location, and imageFile

Events
Go to Event

View and manage event details and assign them to specific venues

---
Future Enhancements
Add authentication and role-based access

Allow file/image uploads for venue images

Search and filter functionality for bookings and events

Export booking reports (PDF/Excel)

Calendar view for bookings

Enjoy the website!!!!

GitHub link:https://github.com/IIEMSA/part-2-poe-MashifaneNeo.git