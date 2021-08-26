using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace VOICEVOX_CSharp_Sample
{
    class Program
    {

        //VoiceVox API (v0.3.1)
        private static readonly string voicevox_baseurl = "http://localhost:50021";
        private static readonly string voicevox_query = voicevox_baseurl + "/audio_query?";
        private static readonly string voicevox_synthesis = voicevox_baseurl + "/synthesis?";
        private static readonly string voicevox_speaker_metan = "speaker=0";
        private static readonly string voicevox_speaker_zunda = "speaker=1";

        static void Main(string[] args)
        {
            if (args.Count() == 0) return;
            string message = args[0];

            Console.WriteLine("Input text: " + message);

            Task task = GenerateVoiceAndPlay(message);
            task.Wait();

        }


        private static async Task GenerateVoiceAndPlay(string message)
        {
            string encodedMessage = System.Net.WebUtility.UrlEncode(message);

            //クエリを作成する
            string query = voicevox_query + "text=" + encodedMessage + "&" + voicevox_speaker_metan;
            string synthesisquery = voicevox_synthesis + voicevox_speaker_metan;

            string queryResult = "";

            using (var client = new HttpClient())
            {
                //生成のためのクエリを発行する
                HttpResponseMessage jsonresponse;
                var content = new StringContent("", Encoding.UTF8, "application/json");

                try
                {
                    jsonresponse = await client.PostAsync(query, content);
                }
                catch (HttpRequestException)
                {
                    Console.WriteLine("VOICEVOX API が正常に起動していません");
                    return;
                }

                queryResult = await jsonresponse.Content.ReadAsStringAsync();

                //audio_queryの結果を受け取ったので念のためJSONを表示
                Console.WriteLine(queryResult);

                //受け取ったクエリを元に、音声合成する
                //クエリをPOSTして音声を生成する

                var synthesiscontent = new StringContent(queryResult, Encoding.UTF8, "application/json");
                var voiceResponse = await client.PostAsync(synthesisquery, synthesiscontent);
                var voicestream = await voiceResponse.Content.ReadAsStreamAsync();

                //音声を再生する
                var soundPlayer = new System.Media.SoundPlayer(voicestream);
                soundPlayer.PlaySync();

            }
        }
    }
}
