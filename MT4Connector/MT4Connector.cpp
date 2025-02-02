// This is the main DLL file.

#include "stdafx.h"

#include "MT4Handler.h"
#include "Windows.h"
#include <vcclr.h>
#include <functional>

using namespace System;
using namespace MTAPIService;
using namespace System::Runtime::InteropServices;
using namespace System::Reflection;
using namespace System::Security::Cryptography; 
using namespace System::Security;
using namespace System::Text;
using namespace System::IO;
using namespace  System::Collections::Generic;
using namespace System::Diagnostics;

#define _DLLAPI extern "C" __declspec(dllexport)

void convertSystemString(wchar_t* dest, String^ src)
{
    pin_ptr<const wchar_t> wch = PtrToStringChars(src);
    memcpy(dest, wch, wcslen(wch) * sizeof(wchar_t));
    dest[wcslen(wch)] = '\0';
}

template <typename T> T Execute(std::function<T()> func, wchar_t* err, T default_value)
{
    T result = default_value;
    try
    {
        result = func();
    }
    catch (Exception^ e)
    {
        convertSystemString(err, e->Message);
        MTAdapter::GetInstance()->LogError(e->Message);
    }
    return result;
}

_DLLAPI bool _stdcall initExpert(int expertHandle, int port, wchar_t* symbol, double bid, double ask, wchar_t* err)
{
    return Execute<bool>([&expertHandle, &port, symbol, &bid, &ask]() {
        auto expert = gcnew MTExpert(expertHandle, gcnew String(symbol), bid, ask, gcnew MT4Handler());
        MTAdapter::GetInstance()->AddExpert(port, expert);
        return true;
    }, err, false);
}

_DLLAPI bool _stdcall deinitExpert(int expertHandle, wchar_t* err)
{
    return Execute<bool>([&expertHandle]() {
        MTAdapter::GetInstance()->RemoveExpert(expertHandle);
        return true;
    }, err, false);
}

_DLLAPI bool _stdcall updateQuote(int expertHandle, wchar_t* symbol, double bid, double ask, wchar_t* err)
{
    return Execute<bool>([&expertHandle, symbol, &bid, &ask]() {
        MTAdapter::GetInstance()->SendQuote(expertHandle, gcnew String(symbol), bid, ask);
        return true;
    }, err, false);
}


_DLLAPI bool _stdcall sendEvent(int expertHandle, int eventType, wchar_t* payload, wchar_t* err)
{
    return Execute<bool>([&expertHandle, &eventType, payload]() {
        MTAdapter::GetInstance()->SendEvent(expertHandle, eventType, gcnew String(payload));
        return true;
    }, err, false);
}

_DLLAPI bool _stdcall sendIntResponse(int expertHandle, int response, wchar_t* err)
{
    return Execute<bool>([&expertHandle, &response]() {
        MTAdapter::GetInstance()->SendResponse(expertHandle, gcnew MTResponseInt(response));
        return true;
    }, err, false);
}

_DLLAPI bool _stdcall sendBooleanResponse(int expertHandle, bool response, wchar_t* err)
{
    return Execute<bool>([&expertHandle, &response]() {
        MTAdapter::GetInstance()->SendResponse(expertHandle, gcnew MTResponseBool(response));
        return true;
    }, err, false);
}

_DLLAPI bool _stdcall sendDoubleResponse(int expertHandle, double response, wchar_t* err)
{
    return Execute<bool>([&expertHandle, &response]() {
        MTAdapter::GetInstance()->SendResponse(expertHandle, gcnew MTResponseDouble(response));
        return true;
    }, err, false);
}

_DLLAPI bool _stdcall sendStringResponse(int expertHandle, wchar_t* response, wchar_t* err)
{
    return Execute<bool>([&expertHandle, response]() {
        MTAdapter::GetInstance()->SendResponse(expertHandle, gcnew MTResponseString(gcnew String(response)));
        return true;
    }, err, false);
}

_DLLAPI bool _stdcall sendErrorResponse(int expertHandle, int code, wchar_t* message, wchar_t* err)
{
    return Execute<bool>([&expertHandle, &code, message]() {
        MTResponseString^ res = gcnew MTResponseString(gcnew String(message));
        res->ErrorCode = code;
        MTAdapter::GetInstance()->SendResponse(expertHandle, res);
        return true;
    }, err, false);
}

_DLLAPI bool _stdcall sendVoidResponse(int expertHandle, wchar_t* err)
{
    return Execute<bool>([&expertHandle]() {
        MTAdapter::GetInstance()->SendResponse(expertHandle, gcnew MTResponseObject(nullptr));
        return true;
    }, err, false);
}

_DLLAPI bool _stdcall sendDoubleArrayResponse(int expertHandle, double* values, int size, wchar_t* err)
{
    return Execute<bool>([&expertHandle, values, &size]() {
        array<double>^ list = gcnew array<double>(size);
        for (int i = 0; i < size; i++)
        {
            list[i] = values[i];
        }
        MTAdapter::GetInstance()->SendResponse(expertHandle, gcnew MTResponseDoubleArray(list));
        return true;
    }, err, false);
}

_DLLAPI bool _stdcall sendIntArrayResponse(int expertHandle, int* values, int size, wchar_t* err)
{
    return Execute<bool>([&expertHandle, values, &size]() {
        array<int>^ list = gcnew array<int>(size);
        for (int i = 0; i < size; i++)
        {
            list[i] = values[i];
        }
        MTAdapter::GetInstance()->SendResponse(expertHandle, gcnew MTResponseIntArray(list));
        return true;
    }, err, false);
}

_DLLAPI bool _stdcall sendLongResponse(int expertHandle, __int64 response, wchar_t* err)
{
    return Execute<bool>([&expertHandle, &response]() {
        MTAdapter::GetInstance()->SendResponse(expertHandle, gcnew MTResponseLong(response));
        return true;
    }, err, false);
}

_DLLAPI bool _stdcall getCommandType(int expertHandle, int* res, wchar_t* err)
{
    return Execute<bool>([&expertHandle, res]() {
        *res = MTAdapter::GetInstance()->GetCommandType(expertHandle);
        return true;
    }, err, false);
}

// --- index parameters

_DLLAPI bool _stdcall getIntValue(int expertHandle, int paramIndex, int* res, wchar_t* err)
{
    return Execute<bool>([&expertHandle, &paramIndex, res]() {
        *res = MTAdapter::GetInstance()->GetCommandParameter<int>(expertHandle, paramIndex);
        return true;
    }, err, false);
}

_DLLAPI bool _stdcall getDoubleValue(int expertHandle, int paramIndex, double* res, wchar_t* err)
{
    return Execute<bool>([&expertHandle, &paramIndex, res]() {
        *res = MTAdapter::GetInstance()->GetCommandParameter<double>(expertHandle, paramIndex);
        return true;
    }, err, false);
}

_DLLAPI bool _stdcall getStringValue(int expertHandle, int paramIndex, wchar_t* res, wchar_t* err)
{
    return Execute<bool>([&expertHandle, &paramIndex, res]() {
        convertSystemString(res, MTAdapter::GetInstance()->GetCommandParameter<String^>(expertHandle, paramIndex));
        return true;
    }, err, false);
}

_DLLAPI bool _stdcall getBooleanValue(int expertHandle, int paramIndex, int* res, wchar_t* err)
{
    return Execute<bool>([&expertHandle, &paramIndex, res]() {
        *res = MTAdapter::GetInstance()->GetCommandParameter<bool>(expertHandle, paramIndex);
        return true;
    }, err, false);
}

_DLLAPI bool _stdcall getLongValue(int expertHandle, int paramIndex, __int64* res, wchar_t* err)
{
    return Execute<bool>([&expertHandle, &paramIndex, res]() {
        *res = MTAdapter::GetInstance()->GetCommandParameter<__int64>(expertHandle, paramIndex);
        return true;
    }, err, false);
}

// --- named parameters

_DLLAPI bool _stdcall containsNamedValue(int expertHandle, wchar_t* paramName)
{
    wchar_t err[1000];
    return Execute<bool>([&expertHandle, paramName]() {
        return MTAdapter::GetInstance()->ContainsNamedParameter(expertHandle, gcnew String(paramName));
    }, err, false);
}

_DLLAPI bool _stdcall getNamedIntValue(int expertHandle, wchar_t* paramName, int* res, wchar_t* err)
{
    return Execute<bool>([&expertHandle, paramName, res]() {
        System::Object^ obj = MTAdapter::GetInstance()->GetNamedParameter(expertHandle, gcnew String(paramName));
        *res = (int)obj;
        return true;
    }, err, false);
}

_DLLAPI bool _stdcall getNamedDoubleValue(int expertHandle, wchar_t* paramName, double* res, wchar_t* err)
{
    return Execute<bool>([&expertHandle, paramName, res]() {
        System::Object^ obj = MTAdapter::GetInstance()->GetNamedParameter(expertHandle, gcnew String(paramName));
        *res = (double)obj;
        return true;
    }, err, false);
}

_DLLAPI bool _stdcall getNamedStringValue(int expertHandle, wchar_t* paramName, wchar_t* res, wchar_t* err)
{
    return Execute<bool>([&expertHandle, paramName, res]() {
        System::Object^ obj = MTAdapter::GetInstance()->GetNamedParameter(expertHandle, gcnew String(paramName));
        convertSystemString(res, (String^)obj);
        return true;
    }, err, false);
}

_DLLAPI bool _stdcall getNamedBooleanValue(int expertHandle, wchar_t* paramName, int* res, wchar_t* err)
{
    return Execute<bool>([&expertHandle, paramName, res]() {
        System::Object^ obj = MTAdapter::GetInstance()->GetNamedParameter(expertHandle, gcnew String(paramName));
        *res = (bool)obj;
        return true;
    }, err, false);
}

_DLLAPI bool _stdcall getNamedLongValue(int expertHandle, wchar_t* paramName, __int64* res, wchar_t* err)
{
    return Execute<bool>([&expertHandle, paramName, res]() {
        System::Object^ obj = MTAdapter::GetInstance()->GetNamedParameter(expertHandle, gcnew String(paramName));
        if (obj != nullptr)
            *res = (__int64)obj;
        return true;
    }, err, false);
}