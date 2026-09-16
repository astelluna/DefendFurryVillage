using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableUpdateManager : MonoBehaviour
{
    public static AddressableUpdateManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    /// <summary>
    /// Addressables 업데이트 확인 및 Preload 콘텐츠 다운로드를 시작합니다.
    /// </summary>
    public void StartUpdateCheck(Action<bool> onComplete)
    {
        StartCoroutine(UpdateRoutine(onComplete));
    }

    private IEnumerator UpdateRoutine(Action<bool> onComplete)
    {
        // =========================================================
        // 1. Addressables 초기화
        // =========================================================

        AsyncOperationHandle initHandle;

        try
        {
            // false = 자동 Release 하지 않음
            initHandle = Addressables.InitializeAsync(false);
        }
        catch (Exception e)
        {
            Debug.LogError($"[Addressables] 초기화 호출 실패\n{e}");
            onComplete?.Invoke(false);
            yield break;
        }

        yield return initHandle;

        if (!initHandle.IsValid())
        {
            Debug.LogError("[Addressables] 초기화 Handle이 무효합니다.");
            onComplete?.Invoke(false);
            yield break;
        }

        if (initHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError(
                $"[Addressables] 초기화 실패 : {initHandle.Status}"
            );

            Addressables.Release(initHandle);
            onComplete?.Invoke(false);
            yield break;
        }

        Debug.Log("[Addressables] 초기화 완료");

        Addressables.Release(initHandle);


        // =========================================================
        // 2. Remote Catalog 업데이트 확인
        // =========================================================

        AsyncOperationHandle<List<string>> checkHandle =
            Addressables.CheckForCatalogUpdates(false);

        yield return checkHandle;

        if (!checkHandle.IsValid())
        {
            Debug.LogError(
                "[Addressables] Catalog 확인 Handle이 무효합니다."
            );

            onComplete?.Invoke(false);
            yield break;
        }

        if (checkHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError(
                $"[Addressables] Catalog 업데이트 확인 실패 : {checkHandle.Status}"
            );

            Addressables.Release(checkHandle);
            onComplete?.Invoke(false);
            yield break;
        }

        List<string> catalogsToUpdate = checkHandle.Result;

        Debug.Log(
            $"[Addressables] 업데이트할 Catalog 수 : {catalogsToUpdate.Count}"
        );


        // =========================================================
        // 3. Catalog 업데이트
        // =========================================================

        if (catalogsToUpdate.Count > 0)
        {
            AsyncOperationHandle<List<IResourceLocator>> updateHandle =
                Addressables.UpdateCatalogs(catalogsToUpdate, false);

            yield return updateHandle;

            if (!updateHandle.IsValid())
            {
                Debug.LogError(
                    "[Addressables] Catalog 업데이트 Handle이 무효합니다."
                );

                Addressables.Release(checkHandle);
                onComplete?.Invoke(false);
                yield break;
            }

            if (updateHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError(
                    $"[Addressables] Catalog 업데이트 실패 : {updateHandle.Status}"
                );

                Addressables.Release(updateHandle);
                Addressables.Release(checkHandle);

                onComplete?.Invoke(false);
                yield break;
            }

            Debug.Log("[Addressables] Catalog 업데이트 완료");

            Addressables.Release(updateHandle);
        }

        Addressables.Release(checkHandle);


        // =========================================================
        // 4. Preload 콘텐츠 다운로드 용량 확인
        // =========================================================

        AsyncOperationHandle<long> sizeHandle =
            Addressables.GetDownloadSizeAsync("Preload");

        yield return sizeHandle;

        if (!sizeHandle.IsValid())
        {
            Debug.LogError(
                "[Addressables] 다운로드 용량 확인 Handle이 무효합니다."
            );

            onComplete?.Invoke(false);
            yield break;
        }

        if (sizeHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError(
                $"[Addressables] 다운로드 용량 확인 실패 : {sizeHandle.Status}"
            );

            Addressables.Release(sizeHandle);
            onComplete?.Invoke(false);
            yield break;
        }

        long downloadSize = sizeHandle.Result;

        Debug.Log(
            $"[Addressables] 다운로드 필요 용량 : " +
            $"{downloadSize / 1024f / 1024f:F2} MB"
        );

        Addressables.Release(sizeHandle);


        // =========================================================
        // 5. 다운로드할 콘텐츠가 없는 경우
        // =========================================================

        if (downloadSize == 0)
        {
            Debug.Log("[Addressables] 다운로드할 콘텐츠가 없습니다.");

            onComplete?.Invoke(true);
            yield break;
        }


        // =========================================================
        // 6. Preload 콘텐츠 다운로드
        // =========================================================

        AsyncOperationHandle downloadHandle =
            Addressables.DownloadDependenciesAsync("Preload");

        while (!downloadHandle.IsDone)
        {
            Debug.Log(
                $"[Addressables] 다운로드 진행률 : " +
                $"{downloadHandle.PercentComplete * 100f:F1}%"
            );

            yield return null;
        }

        if (!downloadHandle.IsValid())
        {
            Debug.LogError(
                "[Addressables] 다운로드 Handle이 무효합니다."
            );

            onComplete?.Invoke(false);
            yield break;
        }

        if (downloadHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError(
                $"[Addressables] 콘텐츠 다운로드 실패 : " +
                $"{downloadHandle.Status}"
            );

            Addressables.Release(downloadHandle);
            onComplete?.Invoke(false);
            yield break;
        }

        Debug.Log("[Addressables] 콘텐츠 다운로드 완료");

        Addressables.Release(downloadHandle);


        // =========================================================
        // 7. 전체 업데이트 완료
        // =========================================================

        Debug.Log("[Addressables] 업데이트 작업 완료");

        onComplete?.Invoke(true);
    }
}