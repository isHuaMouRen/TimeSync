# TimeSync

这个小工具可以设置系统时间为指定Ntp服务器上的时间

## 参数

### `--retry`

需要整数，代表与Ntp服务器通信失败后重试的次数

### `--ntp-server`

需要字符串，目标Ntp服务器的地址

### 示例

```bash
TimeSync.exe --retry 3 --ntp-server ntp.example.com
```

## 兼容性

此程序仅兼容 **`Windows`** ，因为调用了WinAPI来设置系统时间

## 开发周期

从创建项目开始，整个项目的开发仅用了 **一个小时** 左右
