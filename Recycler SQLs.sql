/*
Navicat MySQL Data Transfer

Source Server         : Local
Source Server Version : 50505
Source Host           : localhost:3306
Source Database       : plusemu

Target Server Type    : MYSQL
Target Server Version : 50505
File Encoding         : 65001

Date: 2017-02-25 20:33:07
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for catalog_ecotron_rewards
-- ----------------------------
DROP TABLE IF EXISTS `catalog_ecotron_rewards`;
CREATE TABLE `catalog_ecotron_rewards` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `item_id` int(10) unsigned NOT NULL,
  `reward_level` int(11) unsigned NOT NULL,
  PRIMARY KEY (`id`),
  KEY `item_id` (`item_id`) USING BTREE
) ENGINE=MyISAM AUTO_INCREMENT=27 DEFAULT CHARSET=latin1 ROW_FORMAT=FIXED;

-- ----------------------------
-- Records of catalog_ecotron_rewards
-- ----------------------------
INSERT INTO `catalog_ecotron_rewards` VALUES ('1', '1542', '5');
INSERT INTO `catalog_ecotron_rewards` VALUES ('2', '1547', '4');
INSERT INTO `catalog_ecotron_rewards` VALUES ('3', '1485', '3');
INSERT INTO `catalog_ecotron_rewards` VALUES ('4', '1487', '3');
INSERT INTO `catalog_ecotron_rewards` VALUES ('5', '1503', '3');
INSERT INTO `catalog_ecotron_rewards` VALUES ('6', '1483', '2');
INSERT INTO `catalog_ecotron_rewards` VALUES ('7', '1486', '2');
INSERT INTO `catalog_ecotron_rewards` VALUES ('8', '1501', '2');
INSERT INTO `catalog_ecotron_rewards` VALUES ('9', '1490', '1');
INSERT INTO `catalog_ecotron_rewards` VALUES ('10', '1493', '1');
INSERT INTO `catalog_ecotron_rewards` VALUES ('11', '1495', '1');
INSERT INTO `catalog_ecotron_rewards` VALUES ('12', '1482', '1');
INSERT INTO `catalog_ecotron_rewards` VALUES ('13', '1498', '1');
INSERT INTO `catalog_ecotron_rewards` VALUES ('14', '1504', '1');
INSERT INTO `catalog_ecotron_rewards` VALUES ('15', '1480', '2');
INSERT INTO `catalog_ecotron_rewards` VALUES ('16', '1494', '2');
INSERT INTO `catalog_ecotron_rewards` VALUES ('17', '1500', '2');
INSERT INTO `catalog_ecotron_rewards` VALUES ('18', '1479', '2');
INSERT INTO `catalog_ecotron_rewards` VALUES ('19', '1488', '2');
INSERT INTO `catalog_ecotron_rewards` VALUES ('20', '1505', '2');
INSERT INTO `catalog_ecotron_rewards` VALUES ('21', '1764', '2');
INSERT INTO `catalog_ecotron_rewards` VALUES ('22', '1766', '2');
INSERT INTO `catalog_ecotron_rewards` VALUES ('23', '1767', '2');
INSERT INTO `catalog_ecotron_rewards` VALUES ('24', '1481', '2');
INSERT INTO `catalog_ecotron_rewards` VALUES ('25', '1484', '2');
INSERT INTO `catalog_ecotron_rewards` VALUES ('26', '1499', '2');

INSERT INTO `catalog_pages` (`id`,`parent_id`, `caption`, `order_num`, `page_layout`, `page_strings_1`) VALUES ('912362', '9224', 'Recycler', '1', 'recycler', 'catalog_recycler_headline3');
INSERT INTO `catalog_pages` (`parent_id`, `caption`, `order_num`, `page_layout`, `page_strings_1`) VALUES ('912362', 'Recycler', '1', 'recycler', 'catalog_recycler_headline3');
