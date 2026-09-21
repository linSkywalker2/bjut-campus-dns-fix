# 安全说明

## 支持范围

该工具只面向 Windows 10/11，并仅修改当前检测到的 Wi‑Fi 网卡的 IPv4 DNS 设置。

## 数据处理

程序不会收集、保存或上传任何数据，也不会读取校园网账号或密码。

## 报告问题

请通过 GitHub Issues 报告安全问题，但不要在公开 Issue 中粘贴密码、Cookie、校园卡号或其他敏感信息。

## 发布文件

当前二进制文件未进行 Authenticode 签名。下载后可使用 PowerShell 计算哈希并与仓库中记录的 SHA-256 对照：

```powershell
Get-FileHash .\北工大校园网DNS修复.exe -Algorithm SHA256
```
