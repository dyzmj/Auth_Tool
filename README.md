## 使用

**环境：** `Visual Studio 2022`

**框架：** `.NET Framework 4.7.2`

## 原理 

使用本机多项指标共同组成唯一码，对唯一码进行加密生成机器码

## 认证程序使用流程

1. 获取机器码，将其提交给远端加密程序；
2. 远端加密程序对机器码进行加密授权，并返回授权证书；
3. 将授权证书导入认证程序中，完成认证。



![image-20240822093844896](https://img.dyzmj.top/img202408220938756.png)
