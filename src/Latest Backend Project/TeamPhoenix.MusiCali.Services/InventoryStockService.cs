using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.DataAccessLayer;

public class InventoryStockService
{
    private InventoryStockDAO inventoryStockDAO;
    private RecoverUserDAO recoverUserDAO;

    public InventoryStockService(IConfiguration configuration)
    {
        inventoryStockDAO = new InventoryStockDAO(configuration);
        recoverUserDAO = new RecoverUserDAO(configuration);
    }

    public async Task<HashSet<InventoryStockModel>> RequestInventoryStockList(string userName)
    {
        var userHash = recoverUserDAO.GetUserHash(userName);
        return await inventoryStockDAO.GetStockList(userHash);// replace username with userHash
    }
}
