/*
 Navicat Premium Data Transfer

 Source Server         : localhost_3306
 Source Server Type    : MySQL
 Source Server Version : 50560
 Source Host           : localhost:3306
 Source Schema         : onlinegame

 Target Server Type    : MySQL
 Target Server Version : 50560
 File Encoding         : 65001

 Date: 04/07/2018 19:28:47
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for itemmsg
-- ----------------------------
DROP TABLE IF EXISTS `itemmsg`;
CREATE TABLE `itemmsg`  (
  `itemname` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `itemprice` int(10) NULL DEFAULT NULL,
  `itemdesc` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `itemlevel` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Compact;

-- ----------------------------
-- Table structure for noticemsg
-- ----------------------------
DROP TABLE IF EXISTS `noticemsg`;
CREATE TABLE `noticemsg`  (
  `date` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `notice` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Compact;

-- ----------------------------
-- Records of noticemsg
-- ----------------------------
INSERT INTO `noticemsg` VALUES ('5月20日公告', 'During the event, you can specify a newly created character in the account as a burning character.');
INSERT INTO `noticemsg` VALUES ('6月10日公告', 'When the burning character reaches level 60 and receives a reward, you cannot enjoy the other characters in the account to enjoy the event.');
INSERT INTO `noticemsg` VALUES ('6月30日公告', '6/28 (Thursday) After Maintenance ~ 7/26 (Thursday) Before Maintenance');

-- ----------------------------
-- Table structure for onlinemsg
-- ----------------------------
DROP TABLE IF EXISTS `onlinemsg`;
CREATE TABLE `onlinemsg`  (
  `username` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Compact;

-- ----------------------------
-- Table structure for userdatamsg
-- ----------------------------
DROP TABLE IF EXISTS `userdatamsg`;
CREATE TABLE `userdatamsg`  (
  `username` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `userdata` blob NULL,
  PRIMARY KEY (`username`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Compact;

-- ----------------------------
-- Records of userdatamsg
-- ----------------------------
INSERT INTO `userdatamsg` VALUES ('ABC', 0x0001000000FFFFFFFF01000000000000000C020000003D5365727665722C2056657273696F6E3D312E302E302E302C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D6E756C6C0501000000115365727665722E506C6179657244617461070000000573636F7265056D6F6E6579056C6576656C0664616D616765046B696C6C0377696E046C6F7365000000000000000808080B08080802000000640000006400000000000000000000000000000000000000000000000B);
INSERT INTO `userdatamsg` VALUES ('Xiaoa', 0x0001000000FFFFFFFF01000000000000000C020000003D5365727665722C2056657273696F6E3D312E302E302E302C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D6E756C6C0501000000115365727665722E506C6179657244617461070000000573636F7265056D6F6E6579056C6576656C0664616D616765046B696C6C0377696E046C6F7365000000000000000808080B08080802000000640000006400000000000000000000000000000000000000000000000B);

-- ----------------------------
-- Table structure for usermsg
-- ----------------------------
DROP TABLE IF EXISTS `usermsg`;
CREATE TABLE `usermsg`  (
  `username` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `password` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  PRIMARY KEY (`username`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Compact;

-- ----------------------------
-- Records of usermsg
-- ----------------------------
INSERT INTO `usermsg` VALUES ('ABC', '123456');
INSERT INTO `usermsg` VALUES ('Xiaoa', '123456');
INSERT INTO `usermsg` VALUES ('zhou', '123');

SET FOREIGN_KEY_CHECKS = 1;
