-- Initialize databases
CREATE DATABASE pos_db;
CREATE DATABASE wms_db;

-- Connect to pos_db and create schema
\c pos_db
CREATE SCHEMA IF NOT EXISTS pos;

-- Connect to wms_db and create schema
\c wms_db
CREATE SCHEMA IF NOT EXISTS wms;
