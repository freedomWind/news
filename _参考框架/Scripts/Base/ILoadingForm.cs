/*
*FileName:	ILoadingForm
*Author:	油菜花
*CreateTime:2022/5/24 18:57:58
*Description:
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILoadingForm
{
    float LoadingProgress { get; set; }
    void OpenLoadingForm();
    void FinishLoading();
}
