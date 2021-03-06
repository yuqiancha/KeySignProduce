// MFCLibrary1.cpp: 定义 DLL 的初始化例程。
//

#include "stdafx.h"
#include "MFCLibrary1.h"
#include "IS_Base64.h"
#include "cvm_cert.h"
//#include "sm2p10.h"
//#include "sm2cert.h"
//
#include "MSCUKeyAPI.h"


int usep10 = 1;
int usegen = 1;
int usepin = 0;

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

//
//TODO:  如果此 DLL 相对于 MFC DLL 是动态链接的，
//		则从此 DLL 导出的任何调入
//		MFC 的函数必须将 AFX_MANAGE_STATE 宏添加到
//		该函数的最前面。
//
//		例如: 
//
//		extern "C" BOOL PASCAL EXPORT ExportedFunction()
//		{
//			AFX_MANAGE_STATE(AfxGetStaticModuleState());
//			// 此处为普通函数体
//		}
//
//		此宏先于任何 MFC 调用
//		出现在每个函数中十分重要。  这意味着
//		它必须作为以下项中的第一个语句:
//		出现，甚至先于所有对象变量声明，
//		这是因为它们的构造函数可能生成 MFC
//		DLL 调用。
//
//		有关其他详细信息，
//		请参阅 MFC 技术说明 33 和 58。
//

// CMFCLibrary1App

BEGIN_MESSAGE_MAP(CMFCLibrary1App, CWinApp)
END_MESSAGE_MAP()


// CMFCLibrary1App 构造

CMFCLibrary1App::CMFCLibrary1App()
{
	// TODO:  在此处添加构造代码，
	// 将所有重要的初始化放置在 InitInstance 中
}


// 唯一的 CMFCLibrary1App 对象

CMFCLibrary1App theApp;

const GUID CDECL _tlid = { 0x037a568c,0xfd42,0x4f2e,{0xa4,0x33,0xfa,0xb2,0xc8,0x2b,0xb7,0xa5} };
const WORD _wVerMajor = 1;
const WORD _wVerMinor = 0;


// CMFCLibrary1App 初始化

BOOL CMFCLibrary1App::InitInstance()
{
	CWinApp::InitInstance();

	if (!AfxSocketInit())
	{
		AfxMessageBox(IDP_SOCKETS_INIT_FAILED);
		return FALSE;
	}

	// 将所有 OLE 服务器(工厂)注册为运行。  这将使
	//  OLE 库得以从其他应用程序创建对象。
	COleObjectFactory::RegisterAll();

	return TRUE;
}

int test(int a, int b)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	CString ss;
	ss = "you and me";
	AfxMessageBox(ss);
	return a + b;
}



int Genrootkey(char* str)
{
	int ret;
	CVM_ECP_GEN_OPT opt;

	unsigned char pkey[500];
	int len;
	char obuf[1000] = { 0 };
	int olen;

	opt.pri_file = "SM2Root.prikey";
	opt.pub_file = "SM2Root.pubkey";

	ret = cvm_ecp_gen_key(&opt, pkey, &len, NULL, NULL);
	if (ret)
		AfxMessageBox("生成密钥对失败!");
	else
		AfxMessageBox("生成密钥对成功!\n私钥文件：SM2Root.prikey\n公钥文件：SM2Root.pubkey");

	IS_Base64Encode((char*)pkey, len, (char*)obuf, &olen, true);

	memcpy(str, obuf, olen);

	return 1;
	//	((CEdit*)GetDlgItem(IDC_EDIT_CER))->SetWindowText(obuf);
}





int Genrootp10(char* str, char* sub_name)
{
	int ret;
	CVM_CSR_WRITE_OPT opt;
	char subject_name[200];

	unsigned char creq[500];
	int len;
	char obuf[1000] = { 0 };
	int olen;

	len = sizeof(sub_name);

	opt.ext_sign = 0;
	opt.subject_key = "SM2Root.prikey";
	opt.subject_name = sub_name;
	opt.subject_pwd = NULL;
	opt.key_usage = 0;
	opt.ns_cert_type = 0;
	opt.output_file = "SM2Root.req";

	ret = cvm_csr_write_from_opt(&opt, creq, &len);
	if (ret)
		AfxMessageBox("根证书P10生成失败!");
	else
		AfxMessageBox("根证书P10生成成功!\n申请文件：SM2Root.req");

	IS_Base64Encode((char*)creq, len, (char*)obuf, &olen, true);

	memcpy(str, obuf, olen);

	return 1;
}


//-----------产生根证书----------
//str--传出的字符串
//serial--传入序列号
//not_befor--传入有效期开始
//not_after--传入有效期结束
//sub_name--传入使用者
//usep10--传输是否基于P10生成证书
int Genrootcer(char* str, char* serial, char* not_befor, char* not_after, char* sub_name, int usep10)
{
	int ret;
	CVM_CRT_WRITE_OPT opt;

	unsigned char cert[500];
	int len;
	char obuf[1000] = { 0 };;
	int olen;

	int slen;

	slen = sizeof(sub_name);
	if ((slen > 200) || (slen <= 0))
	{
		AfxMessageBox("使用者名称输入不正确!");
		return -1;
	}

	opt.serial = serial;
	opt.not_befor = not_befor;
	opt.not_after = not_after;
	opt.issuer_key = "SM2Root.prikey";
	opt.issuer_pwd = NULL;
	opt.selfsign = 1;
	if (usep10)
		opt.request_file = "SM2Root.req";
	else
		opt.request_file = NULL;
	opt.subject_name = sub_name;
	opt.subject_pubkey = "SM2Root.pubkey";
	opt.is_ca = 1;
	opt.max_pathlen = -1;
	opt.key_usage = 0;
	opt.ns_cert_type = 0;
	opt.output_file = "SM2Root.cer";

	ret = cvm_crt_write_from_opt(&opt, cert, &len);
	if (ret)
		AfxMessageBox("根证书生成失败!");
	else
		AfxMessageBox("根证书生成成功!\n证书文件：""SM2Root.cer");

	IS_Base64Encode((char*)cert, len, (char*)obuf, &olen, true);

	memcpy(str, obuf, olen);

	return 1;
}


//-----------产生用户密钥对----------
//usegen--产生用户公钥对 or 导出用户公钥
int Genuserkey()
{
	// TODO: 在此添加控件通知处理程序代码
	int ret;
	CString err;
	BYTE pubkey[65];
	UINT plen;
	   
	LPTSTR 		UserPin = "123456";
	int			UserPinLen = 6;
	DWORD		dwRet = 0;

	if (ret = MSC_ConnectReader())
	{
		AfxMessageBox("请插入USBKey!");
		return -1;
	}	

	dwRet = MSC_VerifyUserPIN((BYTE *)UserPin, UserPinLen);
	if (dwRet != 0x9000)
	{
		AfxMessageBox("MSC_VerifyUserPI Failed!!，错误码：" + err);
	}


	ret = MSC_SM2GenKey();

	if (ret != 0x9000)
	{
		err.Format("%08X", ret);
		AfxMessageBox("用户密钥对产生失败，错误码：" + err);
		return -1;
	}

	pubkey[0] = 0x04;
	ret = MSC_SM2ExportPubKey(pubkey + 1, &plen);
	if (ret != 0x9000)
	{
		err.Format("%08X", ret);
		AfxMessageBox("用户公钥导出失败，错误码：" + err);
		return -1;
	}
	ret = cvm_pk_write_pubkey_file("SM2User.pubkey", pubkey, 65);
	if (ret)
	{
		err.Format("%08X", ret);
		AfxMessageBox("用户公钥写入文件失败，错误码：" + err);
		return -1;
	}
	AfxMessageBox("用户公钥写入文件成功\n公钥文件：SM2User.pubkey");
	return 1;

}



//产生用户P10
int Genuserp10(char* str, char* sub_name)
{
	int ret;
	CString err;
	CVM_CSR_WRITE_OPT opt;
	BYTE pubkey[65];
	UINT plen;

	//char subject_name[200];
	int slen;
	unsigned char creq[500];
	int len;
	char obuf[1000] = { 0 };;
	int olen;

	slen = sizeof(sub_name);
	if ((slen > 200) || (slen <= 0))
	{
		AfxMessageBox("使用者名称输入不正确!");
		return -1;
	}


	LPTSTR 		UserPin = "123456";
	int			UserPinLen = 6;
	DWORD		dwRet = 0;

	if (ret = MSC_ConnectReader())
	{
		AfxMessageBox("请插入USBKey!");
		return -1;
	}

	dwRet = MSC_VerifyUserPIN((BYTE *)UserPin, UserPinLen);
	if (dwRet != 0x9000)
	{
		AfxMessageBox("MSC_VerifyUserPI Failed!!，错误码：" + err);
	}




	opt.ext_sign = 1;
	opt.subject_key = "SM2User.pubkey";
	opt.subject_name = sub_name;
	opt.subject_pwd = NULL;
	opt.key_usage = 0;
	opt.ns_cert_type = 0;
	opt.output_file = "SM2User.req";

	//下列函数需要验证pin成功状态
	ret = cvm_csr_write_from_opt(&opt, creq, &len);
	if (ret)
		AfxMessageBox("用户证书P10生成失败!");
	else
		AfxMessageBox("用户证书P10生成成功!\n申请文件：SM2User.req");

	IS_Base64Encode((char*)creq, len, (char*)obuf, &olen, true);

	memcpy(str, obuf, olen);

	return 1;
}

//产生用户证书
int Genusercer(char* str, char* serial, char* not_befor, char* not_after, char* subject_name, int usep10)
{
	int ret;
	CVM_CRT_WRITE_OPT opt;
	CString err;

	unsigned char cert[500];
	int len;
	char obuf[1000] = { 0 };;
	int olen;

	opt.serial = serial;
	opt.not_befor = not_befor;
	opt.not_after = not_after;
	opt.issuer_key = "SM2Root.prikey";
	opt.issuer_pwd = NULL;
	opt.selfsign = 0;
	opt.issuer_crt = "SM2Root.cer";
	if (usep10)
		opt.request_file = "SM2User.req";
	else
		opt.request_file = NULL;
	opt.subject_name = subject_name;
	opt.subject_pubkey = "SM2User.pubkey";
	opt.is_ca = 1;
	opt.max_pathlen = -1;
	opt.key_usage = 0;
	opt.ns_cert_type = 0;
	opt.output_file = "SM2User.cer";

	ret = cvm_crt_write_from_opt(&opt, cert, &len);
	if (ret)
	{
		err.Format("%08X", ret);
		AfxMessageBox("用户证书生成失败，错误码：" + err);
		return -1;
	}
	else
		AfxMessageBox("用户证书生成成功!\n证书文件：""SM2User.cer");

	IS_Base64Encode((char*)cert, len, (char*)obuf, &olen, true);


	memcpy(str, obuf, olen);
	return 1;
}


//导入用户证书
int Importcert(char* ibuf)
{
	int ret;
	int ilen;
	unsigned char cert[3000];
	int clen;

	if (ret = MSC_ConnectReader())
	{
		AfxMessageBox("请插入USBKey!");
		return -1;
	}
	
	LPTSTR 		UserPin = "123456";
	int			UserPinLen = 6;
	DWORD		dwRet = 0;
	dwRet = MSC_VerifyUserPIN((BYTE *)UserPin, UserPinLen);
	if (dwRet != 0x9000)
	{
		AfxMessageBox("MSC_VerifyUserPI Failed!!，Importcert");
	}

	ilen = sizeof(ibuf);

	IS_Base64Decode(ibuf, ilen, (char*)cert, &clen);

	ret = MSC_WriteCert(cert, clen);
	if (ret != 0x9000)
		AfxMessageBox("写入证书失败!");
	else
		AfxMessageBox("写入证书成功!");

	MSC_DisConnectReader();

	return 1;

}







// DllGetClassObject - 返回类工厂

STDAPI DllGetClassObject(REFCLSID rclsid, REFIID riid, LPVOID* ppv)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	return AfxDllGetClassObject(rclsid, riid, ppv);
}


// DllCanUnloadNow - 允许 COM 卸载 DLL

STDAPI DllCanUnloadNow(void)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	return AfxDllCanUnloadNow();
}


// DllRegisterServer - 将项添加到系统注册表

STDAPI DllRegisterServer(void)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	if (!AfxOleRegisterTypeLib(AfxGetInstanceHandle(), _tlid))
		return SELFREG_E_TYPELIB;

	if (!COleObjectFactory::UpdateRegistryAll())
		return SELFREG_E_CLASS;

	return S_OK;
}


// DllUnregisterServer - 将项从系统注册表中移除

STDAPI DllUnregisterServer(void)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	if (!AfxOleUnregisterTypeLib(_tlid, _wVerMajor, _wVerMinor))
		return SELFREG_E_TYPELIB;

	if (!COleObjectFactory::UpdateRegistryAll(FALSE))
		return SELFREG_E_CLASS;

	return S_OK;
}
