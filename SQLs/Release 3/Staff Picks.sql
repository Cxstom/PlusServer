INSERT INTO `navigator_categories` (`id`, `category`, `category_identifier`, `public_name`, `view_mode`, `required_rank`, `category_type`, `search_allowance`, `enabled`, `order_id`) VALUES ('38', 'official_view', 'staffpicks', '', 'THUMBNAIL', '1', 'staff_picks', 'NOTHING', '1', '0');

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for `navigator_staff_picks`
-- ----------------------------
DROP TABLE IF EXISTS `navigator_staff_picks`;
CREATE TABLE `navigator_staff_picks` (
  `room_id` int(11) NOT NULL,
  `image` varchar(75) DEFAULT '',
  PRIMARY KEY (`room_id`),
  UNIQUE KEY `room_id` (`room_id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records of navigator_staff_picks
-- ----------------------------
INSERT INTO `navigator_staff_picks` VALUES ('1', null);