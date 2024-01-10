# L&L Coffee - A Ordering system for customers to order delivery and order at the table.
This project is our final project at the University of Greenwich.

## Overview

The system is a secure web-enabled role-based platform for users to place orders as well as manage the cafe. Including 4 user groups: Admin, employees, store owners and customers.

## Application Image

### Login Form
![Login](https://user-images.githubusercontent.com/105436645/229393213-633020a1-9c5a-4cce-9365-97f599028503.png)

### Home Page
![Home Page](https://user-images.githubusercontent.com/97430041/229363819-55ab0d6f-ceee-4c3b-b460-87b23ffd50ab.png)

## Roles

- **Admin:** Can access to any account.
- **Staff:** Manages the process of placed orders.
- **Store owner:** Can manage the products and view statistical charts.
- **Customer:** Can add products to the cart and then place an order.

## Role-Based Access Control (RBAC)

| Role                         | Permissions                                                                                                                                                                                                     |
| ---------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Admin    | Can manage all roles and accounts. Can access all accounts                                                                                                |
| Staff    | Can manage all placed orders. Can reply to messages from customers using the service at the table.  |
| Store owner | Can manage products, categories, sizes, and prices. Can view all completed orders. Can view charts of revenue and the most popular categories by day, month or year.  |
| Customer    | Can view menu, add to cart, place order with many payment methods, and chat with staff. |
## Features

| Feature               | Description                                                                                                                                                         |
| --------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
|  Menu Viewing  | Customer can see all products in menu page and put them in to their cart.|
|  Search, Sort and Filter products   | Customer can search for specific products by name, sort products by price and name or filter products by size or category.     |
| Various payment methods       | Customer can place their order with one of 3 payment methods: cash, MoMo or VNPay.        |
|Send SMS   | After placing an order, there will be an SMS message sent to the customer to confirm the order has been placed successfully and a text message when the order is ready.|
| E-invoice email   | Customers can optionally enter email to receive an electronic invoice when entering order information.|
| Tracking order          | Customers can track their order by Order ID and their phone number to see order status.  |
| Chat with staff    | Eat-in customers can chat with staff by using SignalR.  |
| Real-time order receiving     | Staff can receive new order with reload page.    |
| Update order status  | Order status can be switched based on real life status by staff.                          |
| Manage products, categories, sizes and price   | CRUD products, category sizes and price is available for store owner account.|
| Manage tables   | Store owner can use CRUD table functions and create new QR code for each table when create table. |
| Download QR code (all/each table)| Store owner can download all available QR code or each QR code at a time.|
| Download CSV and Excel | CSV/Excel includes data of all orders. |
| Role and account manage| Only admin can create new account and assign role for that account. |
## Reports

### Statistics

- Revenue of products by day/month/year
- Revenue of categories by day/month/year
- Top 3 popular products by day/month/year

### Exception Reports

- Place order with empty cart
- Place order without entering information
- Place order without selecting payment methods

## Statistical Analysis

Statistical analysis such as Top 3 popular products by day/month/year is available.

## Technology Stack

The technology stack for this system includes:

- Front-end: CSS, SignalR, HTML, SCSS, JavaScript and Bootstrap 
- Back-end: ASP.NET Core, Entity Framework with dependency injection, and SignalR integration
- Database: SQL Server

## Contributors

Tran Phan Phi Long 
Dinh Thanh Luan
