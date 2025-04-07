CREATE DATABASE EventEaseDB;
USE EventEaseDB;

-- Venue Table
CREATE TABLE Venue (
    VenueId INT PRIMARY KEY IDENTITY(1,1),
    VenueName VARCHAR(255) NOT NULL,
    Location VARCHAR(255) NOT NULL,
    Capacity INT NOT NULL CHECK (Capacity > 0),
    ImageUrl VARCHAR(500)
);

-- Event Table
CREATE TABLE [EventTable] (
    EventId INT PRIMARY KEY IDENTITY(1,1),
    EventName VARCHAR(255) NOT NULL,
    EventDate DATE NOT NULL,
    Description TEXT,
    VenueId INT NOT NULL,
    FOREIGN KEY (VenueId) REFERENCES Venue(VenueId) ON DELETE CASCADE
);

-- Booking Table (Junction Table)
CREATE TABLE Booking (
    BookingId INT PRIMARY KEY IDENTITY(1,1),
    EventId INT NOT NULL,
    VenueId INT NOT NULL,
    BookingDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (EventId) REFERENCES [EventTable](EventId) ON DELETE CASCADE,
    FOREIGN KEY (VenueId) REFERENCES Venue(VenueId),
    CONSTRAINT unique_booking UNIQUE (VenueId, EventId)
);

-- Data Insertion

-- Insert Venues
INSERT INTO Venue (VenueName, Location, Capacity, ImageUrl) VALUES
('Wedding Hall', 'Johannesburg', 500, 'https://www.avianto.co.za/wp-content/uploads/2024/10/0656-24-Oct-BH-grid.jpg'),
('Water Gala', 'Pretoria', 300, 'https://www.pinnacle-schools.com/wp-content/uploads/2024/02/swimming-westville-220224-2-1160x560.jpg');

-- Insert Events
INSERT INTO EventTable (EventName, EventDate, Description, VenueId) VALUES
('Samsung Expo', '2025-04-18', 'A expo for the latest Samsung phone', 1),
('Tech Conference', '2025-06-15', 'A conference for tech enthusiasts.', 2);

-- Insert Bookings
INSERT INTO Booking (EventId, VenueId) VALUES
(1, 1),
(2, 2);