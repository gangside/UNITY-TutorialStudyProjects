using MongoDB.Driver;
using UnityEngine;

public class Mongo {
    private const string MONGO_URI = "mongodb://morm:1234@firstcluster-shard-00-00-rqziy.mongodb.net:27017,firstcluster-shard-00-01-rqziy.mongodb.net:27017,firstcluster-shard-00-02-rqziy.mongodb.net:27017/test?ssl=true&replicaSet=firstCluster-shard-0&authSource=admin&retryWrites=true&w=majority";
    private const string DATABASE_NAME = "Lobby";

    private MongoClient client;
    private MongoServer server;
    private MongoDatabase db;

    public void Init() {
        client = new MongoClient(MONGO_URI);
        server = client.GetServer();
        db = server.GetDatabase(DATABASE_NAME);

        //초기화하고자하는 콜랙션들
        Debug.Log("데이터베이스가 초기화됐습니다");
    }

    public void Shutdown() {
        client = null;
        server.Shutdown();
        db = null;
    }
}
