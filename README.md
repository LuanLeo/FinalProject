# L&L Coffee - An ordering system for customers to order delivery and order at the table.
This project is our final project at the University of Greenwich.

## Overview

The system is a secure web-enabled role-based platform for users to place orders as well as manage the cafe. Including 4 user groups: Admin, employees, store owners, and customers.

## Application Image

### Login Form
![image_2024-01-11_001415196](https://github.com/LuanLeo/FinalProject/assets/117885072/6d91ae55-9da9-498f-84be-7a42444adaf6)


### Home Page
![image](https://github.com/LuanLeo/FinalProject/assets/117885072/0e0987d2-35a1-44da-9645-c988c4655097)

## Roles

- **Admin:** Can access to any account.
- **Staff:** Manages the process of the placed orders.
- **Store owner:** Can manage the products and view statistical charts.
- **Customer:** Can add products to the cart and then place an order.

## Role-Based Access Control (RBAC)

| Role                         | Permissions                                                                                                                                                                                                     |
| ---------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Admin    | Can manage all roles and accounts. Can access all accounts                                                                                                |
| Staff    | Can manage all placed orders. Can reply to messages from customers using the service at the table.  |
| Store owner | Can manage products, categories, sizes, and prices. Can view all completed orders. Can view charts of revenue and the most popular categories by day, month, or year.  |
| Customer    | Can view menu, add to cart, place order with many payment methods, and chat with staff. |
## Features

| Feature               | Description                                                                                                                                                         |
| --------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
|  Menu Viewing  | Customers can see all products on the menu page and put them into their cart.|
|  Search, Sort, and Filter products   | Customers can search for specific products by name, sort products by price and name, or filter products by size or category.     |
| Various payment methods       | Customer can place their order with one of 3 payment methods: cash, MoMo, or VNPay.        |
|Send SMS   | After placing an order, there will be an SMS message sent to the customer to confirm the order has been placed successfully and a text message when the order is ready.|
| E-invoice email   | Customers can optionally enter email to receive an electronic invoice when entering order information.|
| Tracking order          | Customers can track their order by Order ID and their phone number to see order status.  |
| Chat with staff    | Eat-in customers can chat with staff by using SignalR.  |
| Real-time order receiving     | Staff can receive new orders without reloading the page.    |
| Update order status  | Order status can be switched based on real-life status by staff.                          |
| Manage products, categories, sizes, and prices   | CRUD products, category sizes, and prices are available for store owner accounts.|
| Manage tables   | Store owners can use CRUD table functions and create new QR codes for each table when creating a table. |
| Download QR code (all/each table)| Store owner can download all available QR codes or each QR code at a time.|
| Download CSV and Excel | CSV/Excel includes data of all orders. |
| Role and account management | Only the admin can create a new account and assign a role for that account. |
## Reports

### Statistics

- Revenue of products by day/month/year
- Revenue of categories by day/month/year
- Top 3 popular products by day/month/year

### Exception Reports

- Place an order with the empty cart
- Place order without entering information
- Place order without selecting payment methods

## Statistical Analysis

Statistical analysis such as the Top 3 popular products by day/month/year is available.

## Technology Stack

The technology stack for this system includes:

- Front-end: CSS, SignalR, HTML, SCSS, JavaScript and Bootstrap 
- Back-end: ASP.NET Core, Entity Framework with dependency injection, and SignalR integration
- Database: SQL Server

## Contributors

Tran Phan Phi Long 

Dinh Thanh Luan
