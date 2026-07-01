# 📚 Database Documentation: Biblioteca

This document provides an overview of the database structure, table definitions, and core business relationships for the Library Management system.

---

## 🗺️ Entity-Relationship Diagram (ERD)

The database consists of 14 tables organized around four main core domains: **Users & Access**, **Catalog (Books & Authors)**, **Sales & Cart**, and **Marketing (Promotions)**.

---

## 🧱 Database Domains and Tables

### 1. Users & Access Control
Manages system users and their permission levels.

* **Roles:** Defines user permissions (e.g., Admin, Customer).
* **Users:** Stores credentials and profile status. Every user must have a assigned role.
    * *Key Constraints:* `Username` and `Email` must be **UNIQUE**.

### 2. Book Catalog & Structure
The core domain managing books, creators, and classifications.

* **Categories:** Broad classifications (e.g., Fiction, Science).
* **SubCategories:** Specific sub-groups that belong strictly to a single category.
* **Publisher:** Companies responsible for publishing the books.
* **Authors:** Information about book writers.
* **Books:** The central entity storing metadata, pricing, physical file paths (PDFs), and cover image URLs hosted on cloud storage.
* **BookAuthors:** A many-to-many bridge table connecting `Books` and `Authors`. It includes a `Role` column (e.g., Main Author, Co-Author, Illustrator).

### 3. User Engagement & Feedback
Tracks user interactions with the catalog.

* **Reviews:** Stores user ratings ($1$ to $5$ stars) and comments for specific books.
* **Wishlist:** Acts as a saved list where users can bookmark books they want to purchase later.

### 4. Sales & Transactions
Handles the commercial engine of the application.

* **Sales:** Main transaction header tracking who made the purchase, when, and if a main promotion discount was applied.
* **SaleDetails:** Line items for each sale. Tracks the historical `UnitPrice` and `Quantity` of books sold to ensure financial records remain accurate even if book prices change later.

### 5. Marketing & Promotions
Manages discount campaigns.

* **Promotions:** Contains event rules, start/end dates, active states, and discount percentages.
* **BookPromotions:** A many-to-many bridge table that links specific `Promotions` directly to specific `Books`.

---

## 🛡️ Critical Business Rules & Integrity Constraints

When developing the Service Layer, ensure the following constraints are validated before touching the database:

1.  **Catalog Dependency:** A `SubCategory` cannot exist without a valid `CategoryId`. A `Book` cannot exist without a valid `SubCategoryId` and `PublisherId`.
2.  **Many-to-Many Deletions:** You cannot delete a `Book` or an `Author` without cleaning up their active references in `BookAuthors` and `BookPromotions` first.
3.  **Financial Immutability:** `SaleDetails` must store the **snapshot price** of the book at the exact moment of the sale. Do not read the current book price for past sales calculations.
4.  **Date Validations:** For `Promotions`, `StartDate` must always be earlier than `EndDate`.
5.  **Review Limits:** A user can only submit a numeric rating within the boundaries specified by the business rules (e.g., integers from $1$ to $5$).