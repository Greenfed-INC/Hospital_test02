# Hospital_test02

Для настройки проекта нужно будет установить SSMS 20 и так же выполнить запрос на создание БД ( базы данных ) :
CREATE DATABASE Hospital;
GO
USE Hospital;

CREATE TABLE passport 
(
    passport_id INT PRIMARY KEY IDENTITY (1,1),
    passport_series VARCHAR(4) NOT NULL CHECK (passport_series LIKE '[0-9][0-9][0-9][0-9]'),
    passport_number VARCHAR(6) NOT NULL CHECK (passport_number LIKE '[0-9][0-9][0-9][0-9][0-9][0-9]'),
    CONSTRAINT UK_Passport UNIQUE (passport_series, passport_number)
);

CREATE TABLE patient 
(
    id INT PRIMARY KEY IDENTITY (1,1),
    pass_id INT,
    FIO_patient CHAR(50) NOT NULL,
    date_birth DATETIME NOT NULL,
    Number_OMS VARCHAR(17) NOT NULL CHECK (Number_OMS LIKE '[0-9][0-9][0-9][0-9]-[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'),
    Number_Snils VARCHAR(14) NOT NULL CHECK (Number_Snils LIKE '[0-9][0-9][0-9]-[0-9][0-9][0-9]-[0-9][0-9][0-9] [0-9][0-9]'),
    Number_Phone VARCHAR(12) NOT NULL CHECK (Number_Phone LIKE '+7[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'),
    FOREIGN KEY (pass_id) REFERENCES passport(passport_id),
    CONSTRAINT UK_OMS UNIQUE (Number_OMS),
    CONSTRAINT UK_Snils UNIQUE (Number_Snils)
);
После в VS 2022 выполнить редактиорвание App.config и заменить имя сервера на свое , после можно будет проверять проект.
