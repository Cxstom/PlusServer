/*
Navicat MySQL Data Transfer

Source Server         : MySQL
Source Server Version : 50505
Source Host           : localhost:3306
Source Database       : plusemu

Target Server Type    : MYSQL
Target Server Version : 50505
File Encoding         : 65001

Date: 2017-02-26 11:54:09
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for `room_poll_questions`
-- ----------------------------
DROP TABLE IF EXISTS `room_poll_questions`;
CREATE TABLE `room_poll_questions` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `poll_id` int(11) DEFAULT '0',
  `question` text,
  `question_type` enum('radio','checkbox','textbox') DEFAULT 'textbox',
  `series_order` int(11) DEFAULT '1',
  `minimum_selections` int(11) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records of room_poll_questions
-- ----------------------------

-- ----------------------------
-- Table structure for `room_poll_questions_selections`
-- ----------------------------
DROP TABLE IF EXISTS `room_poll_questions_selections`;
CREATE TABLE `room_poll_questions_selections` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `question_id` int(11) DEFAULT '0',
  `text` varchar(45) DEFAULT '',
  `value` varchar(45) DEFAULT '',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records of room_poll_questions_selections
-- ----------------------------

-- ----------------------------
-- Table structure for `room_polls`
-- ----------------------------
DROP TABLE IF EXISTS `room_polls`;
CREATE TABLE `room_polls` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `room_id` int(11) DEFAULT '0',
  `type` enum('poll','question') NOT NULL DEFAULT 'poll',
  `headline` varchar(75) NOT NULL,
  `summary` text NOT NULL,
  `completion_message` text NOT NULL,
  `credit_reward` int(11) NOT NULL DEFAULT '0',
  `pixel_reward` int(11) NOT NULL DEFAULT '0',
  `badge_reward` varchar(15) NOT NULL DEFAULT '',
  `expiry` double NOT NULL DEFAULT '0',
  `enabled` enum('Y','N') NOT NULL DEFAULT 'Y',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records of room_polls
-- ----------------------------

-- ----------------------------
-- Table structure for `user_room_poll_results`
-- ----------------------------
DROP TABLE IF EXISTS `user_room_poll_results`;
CREATE TABLE `user_room_poll_results` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `user_id` int(11) DEFAULT '0',
  `poll_id` int(11) DEFAULT '0',
  `question_id` int(11) DEFAULT '0',
  `answer` text,
  `timestamp` double DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records of user_room_poll_results
-- ----------------------------
