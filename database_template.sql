CREATE DATABASE IF NOT EXISTS `summer_portfolio_project`;
USE `summer_portfolio_project`;

CREATE TABLE IF NOT EXISTS `portfolios` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `surname` tinytext NOT NULL,
  `forename` tinytext NOT NULL,
  `dob` date NOT NULL,
  `location` tinytext NOT NULL,
  `email` tinytext NOT NULL,
  `image_url` text DEFAULT NULL,
  `approved` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`id`)
);

CREATE TABLE IF NOT EXISTS `portfolio_skills` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `portfolio_id` int(11) NOT NULL,
  `name` tinytext NOT NULL,
  `rating` tinyint(4) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `skill_fk` (`portfolio_id`),
  CONSTRAINT `skill_fk` FOREIGN KEY (`portfolio_id`) REFERENCES `portfolios` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `rating_limit` CHECK (`rating` >= 0 and `rating` <= 5)
);

CREATE TABLE IF NOT EXISTS `staff_accounts` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `username` tinytext NOT NULL,
  `password` tinytext NOT NULL,
  `email` tinytext NOT NULL,
  `can_moderate_portfolios` bit(1) NOT NULL DEFAULT b'0',
  `can_modify_portfolios` bit(1) NOT NULL DEFAULT b'0',
  `can_modify_staff_perms` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `username` (`username`) USING HASH
);
