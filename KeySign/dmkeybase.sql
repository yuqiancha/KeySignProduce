/*
 Navicat Premium Data Transfer

 Source Server         : mysql80
 Source Server Type    : MySQL
 Source Server Version : 80012
 Source Host           : localhost:3306
 Source Schema         : dmkeybase

 Target Server Type    : MySQL
 Target Server Version : 80012
 File Encoding         : 65001

 Date: 08/11/2018 10:27:25
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for tableall
-- ----------------------------
DROP TABLE IF EXISTS `tableall`;
CREATE TABLE `tableall`  (
  `OnlyID` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `状态` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `姓名` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `性别` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT '男',
  `年龄` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `手机号` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `身份证号` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `邮箱账号` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `证书类型` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT '新增',
  `安装类型` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `发证日期` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `证书有效期` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `项目名称` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `APPID` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `APP密码` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `所属单位名称` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `所属单位电话` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `所属单位地址` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `产权所属单位` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `备注` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`OnlyID`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of tableall
-- ----------------------------
INSERT INTO `tableall` VALUES ('086677201811082b44', '有效', '赵四', '男', '48', '13821383739', '220181199808086677', 'zhaosi@gmail.com', '补证', '摄像机', '2018/11/08', '2018/11/08至2019/11/08', '四大工程第一期计划', '0123456789abcdef', 'ppjjww0123456789abcdef', '上海煅凿科技', '02124188888', '上海市闵行区瓶安路88号', '华东版权所', '无');
INSERT INTO `tableall` VALUES ('086677201811083ddf', '作废', '赵四', '男', '48', '13821383739', '220181199808086677', 'zhaosi@gmail.com', '新领', '摄像机', '2018/11/08', '2018/11/08至2019/11/08', '四大工程第一期计划', '0123456789abcdef', 'ppjjww0123456789abcdef', '上海煅凿科技', '02124188888', '上海市闵行区瓶安路88号', '华东版权所', '无');
INSERT INTO `tableall` VALUES ('086677201811084b7d', '作废', '赵四', '男', '48', '13821383739', '220181199808086677', 'zhaosi@gmail.com', '补证', '摄像机', '2018/11/08', '2018/11/08至2019/11/08', '四大工程第一期计划', '0123456789abcdef', 'ppjjww0123456789abcdef', '上海煅凿科技', '02124188888', '上海市闵行区瓶安路88号', '华东版权所', '无');

SET FOREIGN_KEY_CHECKS = 1;
