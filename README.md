# 🐱 Loaf’NCatting Mobile

### Cat Café Ordering & Reservation App

Loaf’NCatting Mobile is a modern mobile application designed for **Loaf’NCatting Cat Café**, allowing customers to browse the café menu, reserve tables, explore cat profiles, place food orders, receive notifications, and communicate with the café directly.

The project aims to combine **food ordering, reservation management, and interactive cat café experiences** into one seamless mobile platform.

---

# ✨ Features

## 👤 Authentication & User Management

* Login with email or phone number
* Authentication state persistence
* Role-based access (Customer / Staff / Admin)
* Input validation and error handling

---

## ☕ Menu & Ordering System

### Product / Menu List

* Browse drinks, cakes, combos, and café products
* Product images and pricing
* Search and filter by category
* Real-time availability status

### Product Detail

* Detailed menu information
* Quantity selection
* Add to Cart
* Out-of-stock handling
* Combo or related item suggestions

### Shopping Cart

* View selected items
* Increase/decrease quantity
* Remove products
* Automatic total price calculation
* Empty cart handling

### Checkout & Billing

* Order summary
* Contact information validation
* Payment selection
* Order creation
* Payment record generation
* Cart auto-clear after successful checkout

---

# 🐾 Cat Gallery

Loaf’NCatting introduces a dedicated **Cat Gallery** to enhance the cat café experience.

Features include:

* Cat gallery view
* Cat profile details
* Breed, age, gender
* Personality and friendliness indicators
* Current status:

  * Available
  * Resting
  * Under care
* Search and filtering

Customers can explore cats before visiting and even plan reservations around their favorite cats.

---

# 📅 Reservation & Table Booking

A core feature of the system.

Customers can:

* Reserve tables in advance
* Select:

  * Date
  * Time
  * Number of guests
  * Contact information
  * Notes
* View available tables
* Track reservation status

Reservation states:

* Pending
* Confirmed
* Cancelled
* Completed

The system prevents scheduling conflicts and updates table availability automatically.

---

# 🔔 Notifications

Customers receive real-time updates including:

* Reservation confirmation
* Order preparation updates
* Payment confirmation
* Promotions and announcements

Features:

* Read / unread status
* Notification history
* Context navigation

---

# 🗺️ Store Location & Navigation

Integrated map features allow customers to:

* View café location
* Check address and contact details
* View opening hours
* Launch Google Maps navigation

Optional:

* Current location permission support

---

# 💬 Messaging / Chat Support

Built-in customer support messaging.

Features:

* Customer–café conversation
* Message history
* Timestamp display
* Real or simulated chatbot responses
* FAQ-style replies for:

  * Opening hours
  * Reservation questions
  * Menu recommendations
  * Café policies

---

# 🛠️ Admin / Staff Management

Loaf’NCatting also supports operational management.

## Menu Management

* Add / Edit / Delete products
* Manage pricing and availability

## Cat Management

* Maintain cat profiles
* Update status and personality data

## Order Management

* View orders
* Update order status

## Reservation Management

* Confirm or cancel bookings
* Synchronize with table status

## Table Management

Table states:

* Available
* Reserved
* Occupied
* Maintenance

## Staff Management

* Role assignment
* Account permissions
* Access control

## Reports & Analytics

Basic analytics may include:

* Revenue
* Popular menu items
* Reservation statistics
* Order tracking

---

# 🧠 State Management

The application follows modern state management practices using:

* **Provider**
  or
* **Bloc / Cubit**

Architecture layers:

```text
UI
↓
State (Provider / Bloc)
↓
Repository / Service
↓
REST API
↓
SQL Server Database
```

Managed states include:

* Authentication
* Menu
* Cart
* Order
* Reservation
* Notification
* Chat
* Location
* Table Status
* Admin / Staff

This ensures:

* Reactive UI updates
* Loading / Error / Success states
* Better maintainability
* Separation of business logic from UI

---

# 🏗️ System Architecture

Backend:

* REST API

Database:

* SQL Server

Mobile Application:

* Flutter *(or update if different)*

Architecture:

* Repository Pattern
* State Management (Provider / Bloc)
* Role-based access

---

# 🚀 Demo Flow

Typical customer flow:

1. Login / Register
2. Browse Menu
3. Search and filter products
4. View product details
5. Add items to cart
6. Reserve a table
7. Checkout and place order
8. Receive notifications
9. Explore Cat Gallery
10. View café location
11. Chat with café support

Staff/Admin flow:

1. Login with admin role
2. Manage menu
3. Manage cats
4. Update reservations
5. Process orders
6. Monitor reports

---

# 👨‍💻 Team

**Loaf’NCatting Development Team**

* Huỳnh An Khương — SE193105
* Nguyễn Quốc Huy — SE193304
* Hoàng Lê Thành Đức — SE192335
* Mai Quốc Anh — SE192977

---

# 📌 Future Improvements

Potential enhancements:

* Push Notifications (Firebase)
* Real-time Chat
* Online Payment Gateway
* AI Cat Recommendation
* Loyalty Program
* Reservation Recommendation System
* Advanced Analytics Dashboard

---

# 📄 License

This project is developed for academic and educational purposes.
