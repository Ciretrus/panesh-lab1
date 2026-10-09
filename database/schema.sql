-- DDL Схема БД PostgreSQL для ювелирной мастерской (3НФ)

DROP TABLE IF EXISTS orders;
DROP TABLE IF EXISTS product_types;
DROP TABLE IF EXISTS materials;
DROP TABLE IF EXISTS clients;

-- 1. Таблица клиентов (независимая сущность с наибольшим числом полей)
CREATE TABLE clients (
    id SERIAL PRIMARY KEY,
    last_name VARCHAR(60) NOT NULL,
    first_name VARCHAR(60) NOT NULL,
    middle_name VARCHAR(60),
    phone VARCHAR(20) NOT NULL UNIQUE,
    email VARCHAR(100),
    passport_series_number VARCHAR(20) NOT NULL UNIQUE,
    address VARCHAR(255)
);

-- 2. Таблица материалов
CREATE TABLE materials (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL UNIQUE,
    unit_price NUMERIC(12, 2) NOT NULL CHECK (unit_price > 0)
);

-- 3. Таблица типов изделий
CREATE TABLE product_types (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL UNIQUE,
    base_work_price NUMERIC(12, 2) NOT NULL CHECK (base_work_price >= 0)
);

-- 4. Таблица заказов
CREATE TABLE orders (
    id SERIAL PRIMARY KEY,
    client_id INT NOT NULL REFERENCES clients(id) ON DELETE RESTRICT,
    product_type_id INT NOT NULL REFERENCES product_types(id) ON DELETE RESTRICT,
    material_id INT NOT NULL REFERENCES materials(id) ON DELETE RESTRICT,
    order_date DATE NOT NULL DEFAULT CURRENT_DATE,
    weight_grams NUMERIC(8, 3) NOT NULL CHECK (weight_grams > 0),
    total_price NUMERIC(12, 2) NOT NULL CHECK (total_price > 0),
    status VARCHAR(30) NOT NULL DEFAULT 'Принят'
);
