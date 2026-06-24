-- Create user for aplication
CREATE USER 'EFUser'@'localhost' IDENTIFIED BY 'EF123';

CREATE DATABASE CoreBankingDb;

GRANT ALL PRIVILEGES ON CoreBankingDb.* TO 'EFUser'@'localhost';

