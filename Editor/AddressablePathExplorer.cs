#if ADDRESSABLE_SUPPORT
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using Object = UnityEngine.Object;

namespace JHS.Library.AutoPathGenerator
{
    public class AddressablePathExplorer
    {
        // 모든 어드레서블 에셋 경로 가져오기
        public static IEnumerable<(string address, string assetPath, string groupName)> GetAllAddressables()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            return settings?.groups
                .Where(group => group != null)
                .SelectMany(group => group.entries.Select(entry => (
                    address: entry.address,
                    assetPath: entry.AssetPath,
                    groupName: group.Name
                ))) ?? Enumerable.Empty<(string, string, string)>();
        }

        // 특정 그룹의 어드레서블만 가져오기
        public static IEnumerable<string> GetAddressablesByGroup(string groupName)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            return settings?.groups
                       .Where(group => group != null && group.Name == groupName)
                       .SelectMany(group => group.entries.Select(entry => entry.address))
                   ?? Enumerable.Empty<string>();
        }

        // 특정 타입의 어드레서블만 가져오기
        public static IEnumerable<string> GetAddressablesByType<T>() where T : Object
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            return settings?.groups
                       .Where(group => group != null)
                       .SelectMany(group => group.entries)
                       .Where(entry => AssetDatabase.GetMainAssetTypeAtPath(entry.AssetPath) == typeof(T))
                       .Select(entry => entry.address)
                   ?? Enumerable.Empty<string>();
        }

        // 특정 라벨을 가진 어드레서블 가져오기
        public static IEnumerable<string> GetAddressablesByLabel(string label)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            return settings?.groups
                       .Where(group => group != null)
                       .SelectMany(group => group.entries)
                       .Where(entry => entry.labels.Contains(label))
                       .Select(entry => entry.address)
                   ?? Enumerable.Empty<string>();
        }

        // 그룹별로 어드레서블 그룹화
        public static IEnumerable<IGrouping<string, string>> GetAddressablesGroupedByGroup()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            return settings?.groups
                       .Where(group => group != null)
                       .SelectMany(group => group.entries.Select(entry => (
                           groupName: group.Name,
                           address: entry.address
                       )))
                       .GroupBy(x => x.groupName, x => x.address)
                   ?? Enumerable.Empty<IGrouping<string, string>>();
        }

        // 경로에 특정 키워드가 포함된 어드레서블 검색
        public static IEnumerable<string> SearchAddressables(string keyword)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            return settings?.groups
                       .Where(group => group != null)
                       .SelectMany(group => group.entries)
                       .Where(entry =>
                           entry.address.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                           entry.AssetPath.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                       .Select(entry => entry.address)
                   ?? Enumerable.Empty<string>();
        }
    }
}
#endif