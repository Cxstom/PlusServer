/*
Navicat MySQL Data Transfer

Source Server         : MySQL
Source Server Version : 50505
Source Host           : localhost:3306
Source Database       : plusemu

Target Server Type    : MYSQL
Target Server Version : 50505
File Encoding         : 65001

Date: 2017-02-26 11:57:25
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for `group_forum_posts`
-- ----------------------------
DROP TABLE IF EXISTS `group_forum_posts`;
CREATE TABLE `group_forum_posts` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `thread_id` int(11) NOT NULL DEFAULT '0',
  `content` text NOT NULL,
  `user_id` int(11) NOT NULL DEFAULT '0',
  `created_at` double NOT NULL DEFAULT '0',
  `deleted` enum('0','10') NOT NULL DEFAULT '0',
  `moderator_id` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records of group_forum_posts
-- ----------------------------

-- ----------------------------
-- Table structure for `group_forum_settings`
-- ----------------------------
DROP TABLE IF EXISTS `group_forum_settings`;
CREATE TABLE `group_forum_settings` (
  `group_id` int(11) NOT NULL,
  `readability_setting` enum('0','1','2') NOT NULL DEFAULT '0',
  `post_creation_setting` enum('0','1','2','3') NOT NULL DEFAULT '0',
  `thread_creation_setting` enum('0','1','2','3') NOT NULL DEFAULT '0',
  `moderation_setting` enum('2','3') NOT NULL DEFAULT '2',
  `score` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`group_id`),
  UNIQUE KEY `group_id` (`group_id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records of group_forum_settings
-- ----------------------------

-- ----------------------------
-- Table structure for `group_forum_threads`
-- ----------------------------
DROP TABLE IF EXISTS `group_forum_threads`;
CREATE TABLE `group_forum_threads` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `group_id` int(11) NOT NULL DEFAULT '0',
  `title` varchar(175) NOT NULL DEFAULT '',
  `user_id` int(11) NOT NULL DEFAULT '0',
  `view_count` int(11) NOT NULL DEFAULT '0',
  `created_at` double NOT NULL DEFAULT '0',
  `updated_at` double NOT NULL DEFAULT '0',
  `pinned` enum('0','1') NOT NULL DEFAULT '0',
  `locked` enum('0','1') NOT NULL,
  `deleted` enum('0','10') NOT NULL,
  `moderator_id` int(11) NOT NULL DEFAULT '0',
  `views` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `id` (`id`),
  KEY `group_id` (`group_id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records of group_forum_threads
-- ----------------------------
