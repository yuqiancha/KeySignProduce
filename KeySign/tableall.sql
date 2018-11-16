/*
Navicat MySQL Data Transfer

Source Server         : mysql80
Source Server Version : 80012
Source Host           : localhost:3306
Source Database       : dmkeybase

Target Server Type    : MYSQL
Target Server Version : 80012
File Encoding         : 65001

Date: 2018-11-13 20:45:03
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for tableall
-- ----------------------------
DROP TABLE IF EXISTS `tableall`;
CREATE TABLE `tableall` (
  `证书编号` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT '',
  `姓名` varchar(255) NOT NULL,
  `性别` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT '男',
  `年龄` varchar(255) DEFAULT NULL,
  `手机号` varchar(255) DEFAULT NULL,
  `身份证号` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `邮箱账号` varchar(255) DEFAULT NULL,
  `证书类型` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT '新增',
  `设备类型` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `发证日期` varchar(255) DEFAULT NULL,
  `证书有效期` varchar(255) DEFAULT NULL,
  `项目名称` varchar(255) DEFAULT NULL,
  `APPID` varchar(255) DEFAULT NULL,
  `APP密码` varchar(255) DEFAULT NULL,
  `所属单位名称` varchar(255) DEFAULT NULL,
  `所属单位电话` varchar(255) DEFAULT NULL,
  `所属单位地址` varchar(255) DEFAULT NULL,
  `备注` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `状态` varchar(255) DEFAULT NULL,
  `设备所属单位` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY (`证书编号`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- ----------------------------
-- Records of tableall
-- ----------------------------
INSERT INTO `tableall` VALUES ('086677201811133dc9', '赵四', '男', '48', '13821383739', '220181199808086677', 'zhaosi@gmail.com', '新领', '摄像机', '2018/11/13 20:44:18', '2018/11/13至2021/11/13', '四大工程第一期计划', 'ZSi', '086677', '上海煅凿科技', '021-24188888', '上海市闵行区瓶安路88号', '', '0', '华东版权所');
INSERT INTO `tableall` VALUES ('0866782018111306f6', '赵四', '男', '48', '13821383739', '220181199808086678', 'zhaosi@gmail.com', '新领', '摄像机', '2018/11/13 20:44:18', '2018/11/13至2021/11/13', '四大工程第一期计划', 'ZSi01', '086678', '上海煅凿科技', '021-24188888', '上海市闵行区瓶安路88号', '', '0', '华东版权所');
