# 北工大校园网 DNS 修复

一个面向 Windows 的小工具，用于解决连接北京工业大学校园 Wi‑Fi 后，因无线网卡使用了公共 DNS 而无法打开 `lgn.bjut.edu.cn` 的情况。

> 本项目是个人制作的非官方工具，与北京工业大学无隶属、授权或背书关系。

## 功能

- 自动查找 Windows 中的 Wi‑Fi 网卡
- 将该 Wi‑Fi 网卡的 IPv4 DNS 恢复为自动获取（DHCP）
- 清空 Windows DNS 缓存
- 打开 `http://lgn.bjut.edu.cn/`
- 不关闭、不修改 VPN，也不修改系统代理

## 使用方法

1. 从仓库的 `release` 目录下载 `北工大校园网DNS修复.exe`。
2. 双击运行，并允许 Windows 管理员权限请求。
3. 点击“一键修复并打开登录页”。

修改网络适配器的 DNS 设置需要管理员权限，因此 Windows 会显示 UAC 确认窗口。

## FlClash / Clash 用户

本工具只修复 Windows 无线网卡的 DNS 设置，不会改动 Clash 配置。如果修复后仍无法访问，请在 Clash 配置中将 `lgn.bjut.edu.cn` 或 `*.bjut.edu.cn` 设置为直连，并避免对该域名使用 Fake-IP 或公共 DNS。

## 安全与隐私

工具不会收集、保存或上传账号、密码、网络信息及其他数据。源代码仅执行以下系统操作：

```powershell
Set-DnsClientServerAddress -InterfaceIndex <Wi-Fi 接口编号> -ResetServerAddresses
Clear-DnsClientCache
```

随后使用默认浏览器打开校园网登录页。具体实现可直接查看 [`src/CampusDnsFix.cs`](src/CampusDnsFix.cs)。

## 自行构建

支持安装了 .NET Framework 4.x 的 Windows 10/11，无需安装第三方依赖。在 PowerShell 中运行：

```powershell
.\build.ps1
```

生成文件位于 `release/北工大校园网DNS修复.exe`。

## 数字签名说明

当前发布文件没有 Authenticode 数字签名，因此 Windows 可能显示“未知发布者”。文件属性中的公司/作者信息为“国产林林漆”，但这不等同于受信任的代码签名。若要让 UAC 显示“已验证的发布者”，需要购买并妥善使用受信任 CA 签发的代码签名证书。

## 作者

国产林林漆

## 许可

[MIT License](LICENSE)
