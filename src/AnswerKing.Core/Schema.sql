-- Entities START
CREATE TABLE Category (
    Id INT IDENTITY(1, 1) PRIMARY KEY,
    Name VARCHAR(50) NOT NULL UNIQUE,
);

CREATE TABLE Item (
    Id INT IDENTITY(1, 1) PRIMARY KEY,
    Name VARCHAR(50) NOT NULL UNIQUE,
    Price DECIMAL(19, 4) NOT NULL,
    Description VARCHAR(500),
);

CREATE TABLE OrderStatus (
    Id INT IDENTITY(1, 1) PRIMARY KEY,
    Status VARCHAR(50) NOT NULL
)

CREATE TABLE [Order] (
    Id INT IDENTITY(1, 1) PRIMARY KEY,
    StatusId INT NOT NULL DEFAULT 1,
    Address VARCHAR(500) NOT NULL,
    FOREIGN KEY (StatusId) REFERENCES OrderStatus(Id)
)
-- Entities END

-- Join entities START
CREATE TABLE ItemCategory (
    ItemId INT NOT NULL,
    CategoryId INT NOT NULL,
    FOREIGN KEY (ItemId) REFERENCES Item(Id) ON DELETE CASCADE,
    FOREIGN KEY (CategoryId) REFERENCES Category(Id) ON DELETE CASCADE
);

CREATE TABLE OrderItem (
    ItemId INT NOT NULL,
    OrderId INT NOT NULL,
    Quantity INT NOT NULL DEFAULT 1,
    FOREIGN KEY (ItemId) REFERENCES Item(Id) ON DELETE CASCADE,
    FOREIGN KEY (OrderId) REFERENCES [Order](Id) ON DELETE CASCADE
);
-- Join entities END

-- Seed database with initial items and categories START
INSERT INTO Item (Name, Price, Description) VALUES ('Burger', 2.99, 'Keeping it simple!');
INSERT INTO Item (Name, Price, Description) VALUES ('Cheese Burger', 3.49, 'Not for the lactose intolerant.');
INSERT INTO Item (Name, Price, Description) VALUES ('Bacon Cheese Burger', 4.49, 'Feeling extra piggish?');
INSERT INTO Item (Name, Price, Description) VALUES ('Answer Burger', 2.99, 'The ULTIMATE burger, made with LOVE.');
INSERT INTO Item (Name, Price, Description) VALUES ('Answer Sliders', 1.99, NULL);
INSERT INTO Item (Name, Price, Description) VALUES ('Fries', 0.99, NULL);
INSERT INTO Item (Name, Price, Description) VALUES ('Water', 0.99, NULL);

INSERT INTO Category (Name) VALUES ('Burger');
INSERT INTO Category (Name) VALUES ('Beverage');
INSERT INTO Category (Name) VALUES ('Side');

INSERT INTO ItemCategory (ItemId, CategoryId) VALUES (1, 1);
INSERT INTO ItemCategory (ItemId, CategoryId) VALUES (2, 1);
INSERT INTO ItemCategory (ItemId, CategoryId) VALUES (3, 1);
INSERT INTO ItemCategory (ItemId, CategoryId) VALUES (4, 1);
INSERT INTO ItemCategory (ItemId, CategoryId) VALUES (5, 3);
INSERT INTO ItemCategory (ItemId, CategoryId) VALUES (5, 1);
INSERT INTO ItemCategory (ItemId, CategoryId) VALUES (6, 3);
INSERT INTO ItemCategory (ItemId, CategoryId) VALUES (7, 2);

INSERT INTO OrderStatus (Status) VALUES ('Pending');
INSERT INTO OrderStatus (Status) VALUES ('Completed');
INSERT INTO OrderStatus (Status) VALUES ('Cancelled');

INSERT INTO [Order] (StatusId, Address) VALUES (1, 'Answer Digital, Union Mills, 9 Dewsbury Rd, Leeds LS11 5DD');

INSERT INTO OrderItem (ItemId, OrderId, Quantity) VALUES (4, 1, 1);
INSERT INTO OrderItem (ItemId, OrderId, Quantity) VALUES (6, 1, 1);
INSERT INTO OrderItem (ItemId, OrderId, Quantity) VALUES (7, 1, 1);
-- Seed database with initial items and categories END