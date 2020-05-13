using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization.Attributes;

namespace WFCrudMongo
{
    public class Produto
    {
        [BsonId()]
        public ObjectId Id { get; set; }

        [BsonElement("nomeproduto")]
        [BsonRequired()]
        public string NomeProduto { get; set; }

        [BsonElement("descricao")]
        [BsonRequired()]
        public string Descricao { get; set; }

        [BsonElement("datacadastro")]
        [BsonRequired()]
        public DateTime DataCadastro { get; set; }

        [BsonElement("ativo")]
        [BsonRequired()]
        public bool Ativo { get; set; }

        [BsonElement("valor")]
        [BsonRequired()]
        public double Valor { get; set; }

        [BsonElement("posicao")]
        [BsonRequired()]
        public int Posicao { get; set; }

        [BsonElement("foto")]
        [BsonRequired()]
        public ObjectId DocumentoIncorporado { get; set; }
    }
}
