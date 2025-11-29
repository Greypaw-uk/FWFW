public static class NetworkItemConverter
{
    public static NetworkItem ToNetworkItem(Items.Item item, string iconPath)
    {
        return new NetworkItem
        {
            Name = item.Name,
            Weight = item.Weight,
            Price = item.Price,
            IconPath = iconPath
        };
    }
}

