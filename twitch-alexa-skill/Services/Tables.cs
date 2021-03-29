using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using twitch_alexa_skill.Entities;
using twitch_alexa_skill.StateUtils;

namespace twitch_alexa_skill.Services
{
    public static class Tables
    {

        public static CloudStorageAccount CreateStorageAccountFromConnectionString(string storageConnectionString)
        {
            CloudStorageAccount storageAccount;
            try
            {
                storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            }
            catch (FormatException e)
            {
                throw e;
            }
            catch (ArgumentException e)
            {

                throw e;
            }

            return storageAccount;
        }
        public static CloudTable GetTable(string tableName)
        {
            var storageAccount = CreateStorageAccountFromConnectionString(Environment.GetEnvironmentVariable("TABLE_CONNECTION_STRING"));
            var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            var table = tableClient.GetTableReference(tableName);

            return table;
        }

        public static async Task<UserEntity> InsertUserAsync(UserEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            CloudTable table = null;
            try
            {
                table = GetTable("Users");
                TableOperation insertOperation = TableOperation.Insert(entity);
                TableResult result = await table.ExecuteAsync(insertOperation);
                UserEntity insertedUser = result.Result as UserEntity;

                return insertedUser;
            }
            catch (Exception e)
            {
                var error = new ErrorEntity(e);
                TableOperation insertOperation = TableOperation.Insert(error);
                await table.ExecuteAsync(insertOperation);
                throw e;
            }
        }

        public static async Task<bool> InsertStateAsync(string alexa_id, string twitch_id)
        {

            CloudTable table = null;
            try
            {
                var entity = new StateEntity();
                entity.PersistentUserState = JsonConvert.SerializeObject(StateCache.Cache[alexa_id].UserContext.State);
                entity.RowKey = alexa_id;
                entity.PartitionKey = twitch_id;

                table = GetTable("States");
                TableOperation insertOperation = TableOperation.Insert(entity);
                TableResult result = await table.ExecuteAsync(insertOperation);
                StateEntity stateEntity = result.Result as StateEntity;

                return true;
            }
            catch (Exception e)
            {
                var error = new ErrorEntity(e);
                table = GetTable("Errors");
                TableOperation insertOperation = TableOperation.Insert(error);
                await table.ExecuteAsync(insertOperation);
                return false;
            }
        }

        public static async Task<bool> InsertRedemptionAsync(RedemptionEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            CloudTable table = null;
            try
            {
                table = GetTable("Redemptions");
                TableOperation insertOperation = TableOperation.Insert(entity);
                await table.ExecuteAsync(insertOperation);
                return true;
            }
            catch (Exception e)
            {
                var error = new ErrorEntity(e);
                TableOperation insertOperation = TableOperation.Insert(error);
                await table.ExecuteAsync(insertOperation);
                return false;
            }
        }

        public static async Task<int> InsertRedemptionsAsync(List<RedemptionEntity> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException("entity");
            }

            CloudTable table = null;
            try
            {
                var tasks = new List<Task>();

                table = GetTable("Redemptions");
                foreach (var entity in entities)
                {
                    TableOperation insertOperation = TableOperation.Insert(entity);
                    tasks.Add(table.ExecuteAsync(insertOperation));
                }

                await Task.WhenAll(tasks);

                return tasks.Count();
            }
            catch (Exception e)
            {
                var error = new ErrorEntity(e);
                TableOperation insertOperation = TableOperation.Insert(error);
                await table.ExecuteAsync(insertOperation);
                return 0;
            }
        }

        public static async Task<int> InsertRewardsAsync(List<RewardEntity> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException("entity");
            }

            CloudTable table = null;
            try
            {

                var tasks = new List<Task>();

                table = GetTable("Rewards");
                foreach (var entity in entities)
                {
                    TableOperation insertOperation = TableOperation.Insert(entity);
                    tasks.Add(table.ExecuteAsync(insertOperation));
                }

                await Task.WhenAll(tasks);

                return tasks.Count();
            }
            catch (Exception e)
            {
                var error = new ErrorEntity(e);
                TableOperation insertOperation = TableOperation.Insert(error);
                await table.ExecuteAsync(insertOperation);
               return 0;
            }
        }

        public static async Task<bool> InsertErrorAsync(ErrorEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            try
            {
                var table = GetTable("Errors");
                TableOperation insertOperation = TableOperation.Insert(entity);
                TableResult result = await table.ExecuteAsync(insertOperation);
                ErrorEntity insertedUser = result.Result as ErrorEntity;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static async Task<UserEntity> GetUser(string alexa_id, string twitch_id)
        {

            CloudTable table = null;
            try
            {
                table = GetTable("Users");
                TableOperation get = TableOperation.Retrieve(alexa_id, twitch_id);
                TableResult result = await table.ExecuteAsync(get);
                UserEntity user = result.Result as UserEntity;

                return user;
            }

            catch (Exception e)
            {
                var error = new ErrorEntity(e);
                TableOperation insertOperation = TableOperation.Insert(error);
                await table.ExecuteAsync(insertOperation);
                throw e;
            }
        }

        public static async Task<UserEntity> GetUserByAlexaId(string alexa_id) {

            CloudTable table = null;
            try
            {
                table = GetTable("Users");
                var user = table.CreateQuery<UserEntity>()
                    .Where(x => x.RowKey == alexa_id)
                    .Select(x => x).ToList();
                if (user.Any())
                {
                    return user.FirstOrDefault();
                }
                return null;
            }

            catch (Exception e)
            {
                var error = new ErrorEntity(e);
                TableOperation insertOperation = TableOperation.Insert(error);
                await table.ExecuteAsync(insertOperation);
                throw e;
            }
        }

        public static async Task<UserEntity> GetUserByTwitchId(string twitch_id)
        {

            CloudTable table = null;
            try
            {
                table = GetTable("Users");
                var user = table.CreateQuery<UserEntity>()
                    .Where(x => x.PartitionKey == twitch_id)
                    .Select(x => x).ToList();

                return user.FirstOrDefault();
            }
            catch (Exception e)
            {
                var error = new ErrorEntity(e);
                TableOperation insertOperation = TableOperation.Insert(error);
                await table.ExecuteAsync(insertOperation);
                throw e;
            }
        }


        public static async Task<List<RewardEntity>> GetRewardsByTwitchUser(string twitch_id)
        {
            CloudTable table = null;
            try
            {
                table = GetTable("Rewards");
                var reward = table.CreateQuery<RewardEntity>()
                    .Where(x => x.PartitionKey == twitch_id)
                    .Select(x => x).ToList();

                return reward;
            }
            catch (Exception e)
            {
                var error = new ErrorEntity(e);
                TableOperation insertOperation = TableOperation.Insert(error);
                await table.ExecuteAsync(insertOperation);
                throw e;
            }
        }

        public static async Task<List<RedemptionEntity>> GetRedemptionByTwitchUser(string twitch_id)
        {

            CloudTable table = null;
            try
            {
                table = GetTable("Redemptions");
                var redemptions = table.CreateQuery<RedemptionEntity>()
                    .Where(x => x.PartitionKey == twitch_id)
                    .Select(x => x).ToList();

                return redemptions;
            }
            catch (StorageException e)
            {
                var error = new ErrorEntity(e);
                TableOperation insertOperation = TableOperation.Insert(error);
                await table.ExecuteAsync(insertOperation);
                throw e;
            }
        }

        public static async Task<bool> GetUserState(string alexa_id, string twitch_id)
        {

            CloudTable table = null;
            try
            {
                table = GetTable("States");
                var retrieveOperation = TableOperation.Retrieve<StateEntity>(twitch_id, alexa_id);
                var result = await table.ExecuteAsync(retrieveOperation);
                StateEntity state = result.Result as StateEntity;
                if(state == null ) { return false; }
                var stateDict = JsonConvert.DeserializeObject<ConcurrentDictionary<string, object>>(state.PersistentUserState);
                StateCache.Cache[alexa_id].UserContext.State = new Dictionary<string, object>(stateDict);

                return true;
            }
            catch (Exception e)
            {
                var error = new ErrorEntity(e);
                table = GetTable("Error");
                TableOperation insertOperation = TableOperation.Insert(error);
                await table.ExecuteAsync(insertOperation);
                return false;
            }
        }

        public static async Task<UserEntity> UpdateUser(UserEntity user)
        {
            CloudTable table = null;
            try
            {
                table = GetTable("Users");
                var update = TableOperation.InsertOrMerge(user);
                TableResult result = await table.ExecuteAsync(update);
                UserEntity userEntity = result.Result as UserEntity;
                if (userEntity != null)
                {
                    return userEntity;
                }
                return null;
            }
            catch (Exception e)
            {
                var error = new ErrorEntity(e);
                table = GetTable("Error");
                TableOperation insertOperation = TableOperation.Insert(error);
                await table.ExecuteAsync(insertOperation);
                return null;
            }
        }
        public static async Task<bool> UpdateUserState(string alexa_id, string twitch_id)
        {
            CloudTable table = null;
            try
            {
                var entity = new StateEntity();
                entity.PersistentUserState = JsonConvert.SerializeObject(StateCache.Cache[alexa_id].UserContext.State);
                entity.RowKey = alexa_id;
                entity.PartitionKey = twitch_id;

                table = GetTable("States");
                var update = TableOperation.InsertOrMerge(entity);               
                TableResult result = await table.ExecuteAsync(update);
                StateEntity stateEntity = result.Result as StateEntity;

                return true;
            }
            catch (Exception e)
            {
                var error = new ErrorEntity(e);
                table = GetTable("Error");
                TableOperation insertOperation = TableOperation.Insert(error);
                await table.ExecuteAsync(insertOperation);
                return false;
            }
        }

        public static async Task<bool> RemoveRewardsAsync(List<RewardEntity> entities) 
        {
            CloudTable table = null;
            try
            {
                table = GetTable("Rewards");
                var tasks = new List<Task>();
                foreach (var entity in entities)
                {
                    var delete = TableOperation.Delete(entity);
                    tasks.Add(table.ExecuteAsync(delete));
                    
                }

                await Task.WhenAll(tasks);
                return true;
            }
            catch (Exception e)
            {
                var error = new ErrorEntity(e);
                table = GetTable("Error");
                TableOperation insertOperation = TableOperation.Insert(error);
                await table.ExecuteAsync(insertOperation);
                return false;
            }
        }

        public static async Task<bool> RemoveRedemptionsAsync(List<RedemptionEntity> entities)
        {
            CloudTable table = null;
            try
            {
                table = GetTable("Redemptions");
                var tasks = new List<Task>();
                foreach (var entity in entities)
                {
                    var delete = TableOperation.Delete(entity);
                    tasks.Add(table.ExecuteAsync(delete));

                }

                await Task.WhenAll(tasks);
                return true;
            }
            catch (Exception e)
            {
                var error = new ErrorEntity(e);
                table = GetTable("Error");
                TableOperation insertOperation = TableOperation.Insert(error);
                await table.ExecuteAsync(insertOperation);
                return false;
            }
        }
    }
}
