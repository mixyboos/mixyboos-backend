using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MixyBoos.Api.Data.Models;

namespace MixyBoos.Api.Data.Seeders {
    public class TestData {
        const int USER_COUNT = 10;
        const int MAX_MIX_COUNT = 20;

        private static readonly string[] MIXES = {
            "8ebefaaa-49cd-4caf-0275-08d5c4fc3fd4",
            "91848ecb-c8ad-41d2-7122-08d5c7e228a9",
            "5bfdc3d5-db4d-47e0-05b6-08d62d8b4050",
            "fa123a62-b3a6-4d53-41da-08d62fbf9136",
            "b85fa547-7b2e-49db-a3f2-08d6ffb1f98d",
            "f17dcaa5-6eb2-491e-5892-08d74901525d",
            "91233c32-e7e7-4211-58ac-08d74901525d",
            "d4c73598-d973-4e4b-af6b-08d763274d2f",
            "dfa57623-0757-4c9e-7a6a-08d64e8c9a34",
            "39d530d9-e8b0-46ff-716a-08d65a24be3d",
            "07886214-74a8-44ce-7198-08d65a24be3d",
            "499252d3-dcfd-47dc-1a0d-08d664643392",
            "681fb9b8-c69d-424d-04bd-08d667b269cb",
            "2a301b28-ea27-4cb4-520c-08d6d7d01a5e",
            "eab42805-68e3-48f7-3666-08d6ed23f75b",
            "d6798080-3134-447f-b302-08d6f269a222",
            "e9898d82-5fa3-4446-62aa-08d726e66022",
            "4c676f1c-04ab-4b9c-58b1-08d74901525d",
            "3173fa19-6e77-4ef4-8a00-08d76c79ccd8",
            "267674df-c263-4c86-8b4b-08d7ad67bc5a",
            "55cde011-6bc3-4f58-8b4c-08d7ad67bc5a",
            "f2e553e1-7edb-424b-8b4d-08d7ad67bc5a",
            "6a5d912d-7b1c-4dde-34be-08d888d6cd8e",
            "1d44e965-5530-43a3-2ee5-08d8d044acc9",
            "f45ca4e8-8adc-4592-2ee6-08d8d044acc9",
            "b832a646-8dfc-4814-2f03-08d8d044acc9",
            "d7240919-eec8-454b-afa2-08d658a68cce",
            "6e7264ac-6865-4657-3bfc-08d6f4c077bc",
            "71d5222f-c5ab-4c60-3bfd-08d6f4c077bc",
            "4fd1e9c7-a572-44d7-d767-08d8dc427db2",
            "5e89613a-c59d-4a3d-671a-08d8daca3168",
            "4e356180-4da7-4586-0873-08d6653d2048",
            "d70b31d6-9c9a-4787-921f-08d68e3285c7",
            "44c3c133-900e-49c2-666b-08d72c897f8b",
            "a1c2ace2-54f3-4f53-666c-08d72c897f8b",
            "e3c6c432-b9cb-48b3-666d-08d72c897f8b",
            "b5a0358d-21e9-428f-5717-08d762e30b23",
            "36d4e820-0ccc-449d-c2ea-08d79f9d66f7",
            "0880146d-2b87-4d07-c2ec-08d79f9d66f7",
            "3afb225e-8ba2-46c5-fc5c-08d7a37a1015",
            "ed992606-018d-40f4-fc5d-08d7a37a1015",
            "e7110d13-8320-4e2b-fc5f-08d7a37a1015",
            "08ad6b76-86ad-45ed-fc60-08d7a37a1015",
            "d4fd7738-a7e3-4def-fc61-08d7a37a1015",
            "d2eff823-ec01-48b1-fc62-08d7a37a1015",
            "ed87efe9-f1fe-46c9-3a1b-08d7a9fff3c2",
            "9ea5ee8d-256c-4aa7-ac41-08d7b1b5e54a",
            "43ce501d-71b9-4cfb-ac42-08d7b1b5e54a",
            "a82febc1-584f-4bfa-ac43-08d7b1b5e54a",
            "4d6c80da-4604-4594-ac44-08d7b1b5e54a",
            "130d975b-9430-47e4-ac45-08d7b1b5e54a",
            "8e81a7a9-6173-4d35-ac46-08d7b1b5e54a",
            "870e19fa-cf04-46bc-ed3f-08d7ba65660e",
            "276a73ed-22a5-4cc2-ed46-08d7ba65660e",
            "5d798f00-a75d-430c-34a8-08d7cf48e0ee",
            "fe3b3a3e-0b91-4585-34b1-08d7cf48e0ee",
            "4646aaa4-2fde-41b3-e096-08d7e243eba7",
            "3ae7aa11-4104-4bb2-e0d3-08d7e243eba7",
            "08405aaf-abce-46de-0d7e-08d825210584",
            "d9f52f2f-ce2c-459e-6f4c-08d8311e22d7",
            "c033dd55-5528-4e31-a6b3-08d871d90171",
            "e2b797c1-7dfd-4374-5088-08d879ea80c5",
            "28bbdf2a-0324-49e2-35fa-08d87e25db67",
            "24df1c59-1112-4516-35fc-08d87e25db67",
            "356c1663-8cde-4341-34aa-08d888d6cd8e",
            "3a33088f-bcf7-473f-34b8-08d888d6cd8e",
            "6d39e2f8-77e1-4116-2668-08d8a21178e7",
            "c0871304-7565-4854-2669-08d8a21178e7",
            "d11301f4-23c6-4f7b-266a-08d8a21178e7",
            "3cd7f5b8-bb4a-406c-074e-08d8a5f207aa",
            "9f491ceb-1017-41ed-074f-08d8a5f207aa",
            "bfad57b8-22c3-4882-e460-08d8cecf8680",
            "cc67db23-3e03-4c52-d764-08d8dc427db2",
            "6eaadbce-8a51-4bfe-d790-08d8dc427db2",
            "b494ec79-8b64-401b-2a06-08d6df5853e6",
            "5e86b3e5-5a49-45d8-65c5-08d715e09330",
            "47764fda-eacf-41df-3f2a-08d76d363466",
            "fde16ed1-2b56-4b38-7369-08d76d58dfa1",
            "925fb52b-aba6-4b8d-b1f7-08d77f33495b",
            "68239992-93a4-4ee3-34b4-08d888d6cd8e",
            "63fe8c2c-32fc-436e-3eb7-08d73df91f82",
            "9a30b987-d088-41bf-3eb8-08d73df91f82",
            "4a736a73-25da-4fdd-3eb9-08d73df91f82",
            "aadb4ce1-b3cf-4b93-02a2-08d768f2c8b2",
            "8503b8b3-561e-4654-02a3-08d768f2c8b2",
            "78c65b46-f270-4beb-26ac-08d795bf711a",
            "2387dfd1-b95e-4338-d18d-08d7963119c5",
            "63527660-4672-492b-01a7-08d799406330",
            "5a2e529f-74a1-4b44-17f7-08d816bf9672",
            "1667255c-476d-4811-d7a2-08d8dc427db2",
            "7070725f-955c-4eac-190c-08d782416dca",
            "21e57b8e-ad8b-4f92-ea9a-08d79a48dcec",
            "a024bdcd-7e59-4b29-3a1a-08d7a9fff3c2",
            "a8c24db2-cf61-41ca-ac57-08d7b1b5e54a",
            "d19332c3-0ffb-44d7-ed4c-08d7ba65660e",
            "d58f30fc-ff01-4387-ed50-08d7ba65660e",
            "b7b5e963-2ab6-4dec-6dd7-08d7c2049978",
            "d8b1c414-ecf2-4dc7-1288-08d7cd147478",
            "d47cbdbb-88b9-43e9-34b4-08d7cf48e0ee",
            "47fb4df6-9261-4ee8-34dc-08d7cf48e0ee",
            "92716a09-c7ff-48ba-e095-08d7e243eba7",
            "e4a8c219-1537-47ba-56e9-08d80f71f484",
            "0d9d05f6-ca00-4498-41d8-08d821b19c64",
            "b275fddc-3a82-4f2e-b3cb-08d843d0a20b",
            "ee611ab3-5662-43bf-7954-08d84c69c483",
            "35900702-b361-4dc5-a6b2-08d871d90171",
            "9b0097a6-e85f-4ced-34bc-08d888d6cd8e",
            "a76455d8-63fd-4045-0749-08d8a5f207aa",
            "6be145f6-b5cb-420e-074a-08d8a5f207aa",
            "183c17ac-f8d8-439b-2f04-08d8d044acc9",
            "49d133bd-972a-409d-2f05-08d8d044acc9",
            "c926b127-70b7-4463-2f06-08d8d044acc9",
            "46deb725-4884-4b05-35f5-08d87e25db67",
            "a7651233-023b-4fd3-35f6-08d87e25db67",
            "656390fe-a393-42df-35f7-08d87e25db67",
            "1367e516-d03e-4bb9-0369-08d8bcc9656b",
            "399a9477-1045-4fab-036c-08d8bcc9656b",
            "fdf1dca4-244d-4ece-037c-08d8bcc9656b",
            "17a8d3d9-880e-444a-037d-08d8bcc9656b",
            "d65c1a4a-39a4-4c7e-7847-08d5c81adceb",
            "e547b935-f0fa-49e7-7848-08d5c81adceb",
            "8c8eefe3-4913-46f8-3f0e-08d5e5e83a2f",
            "5e9c5fe8-75fd-4e4b-41ba-08d62fbf9136",
            "f6085efc-dad2-4f3f-5bfb-08d6a0d922da",
            "9a1eaf01-5a75-4e69-2707-08d6d0352391",
            "c12ac60d-9339-43f1-c6d3-08d6e09bdd53",
            "47d667a6-1973-41fb-ef47-08d7846d75d6",
            "2cd59e40-3743-4206-d18e-08d7963119c5",
            "cb30ff1b-6f91-4742-d18f-08d7963119c5",
            "7f0d7b23-828d-46ce-f39d-08d7c4d670e2",
            "9d4a7910-c4a5-4841-fd6c-08d7f1e47184",
            "348b9b5b-e70b-48d8-e9b5-08d5d1d0919b",
            "aeaae685-7977-40b1-e9b6-08d5d1d0919b",
            "d184e3a7-ade8-48ce-e1e6-08d5da9bd6e9",
            "7fe07a7e-4c15-4d66-e1e7-08d5da9bd6e9",
            "6fbc1c2a-99cc-49f5-e1e8-08d5da9bd6e9",
            "40ac9671-24c5-4fc4-bdd2-08d5e8648302",
            "8319a44c-8575-4b34-8085-08d5f3e0f951",
            "5ef2384b-c341-4d34-edcd-08d5f66a3e89",
            "6caf97b4-d0e4-4fff-3c67-08d5f7f4eec3",
            "19707eaf-f8dc-49c9-8bd2-08d5fc9bf2a7",
            "df305c31-af93-448a-69ec-08d60db73d26",
            "b1993af3-3217-485e-69ed-08d60db73d26",
            "d836f02f-3846-419b-308f-08d6111b2997",
            "b6620a23-7017-47a4-7217-08d618eb39bf",
            "a8f91a4f-bb86-46d6-7218-08d618eb39bf",
            "31905d23-e6b8-4506-7219-08d618eb39bf",
            "5378bdac-3f89-4e64-6001-08d61b54cb7b",
            "a4f0233b-0b74-4807-01f2-08d61facf3af",
            "f4b3a31b-ca61-495b-0204-08d61facf3af",
            "269bd8ca-708e-4831-41d1-08d62fbf9136",
            "01a231c9-e86c-4ac8-d5e1-08d641f99182",
            "9d3294e6-cf55-4ab5-1e09-08d6442bf9dd",
            "39c3df2d-d2f8-4549-1e6d-08d6442bf9dd",
            "6a84e4ae-08ec-4376-3cd3-08d649d7c956",
            "d04aef7e-bd77-4f2b-0484-08d667b269cb",
            "ff939511-e831-4d15-8701-08d6897bbd34",
            "953e60a4-1e7c-4912-beff-08d6b23380f1",
            "273ee7fd-19ba-4d0a-2b4c-08d6b9f495fc",
            "0bb71ce9-67fd-4e70-c760-08d6e09bdd53",
            "4dde252e-4f40-4a62-c761-08d6e09bdd53",
            "44a5c3f4-1369-47fe-c762-08d6e09bdd53",
            "153e239d-1b45-4221-c763-08d6e09bdd53",
            "0f307a5e-ad4c-45f6-1101-08d7167e88d4",
            "8a8a3e3a-50ba-44c0-4a2b-08d7387a9464",
            "982a4490-9561-413f-e598-08d773a220fb",
            "fab92534-283f-418d-051e-08d7777faa61",
            "f2f8d5cd-c4e4-424b-42cb-08d77e2bf6bc",
            "859092e4-357e-4f00-ac5a-08d7b1b5e54a",
            "3f21b795-58de-415c-afdc-08d7b5855a54",
            "203044e2-41a7-4cf3-cbb7-08d7f91fcca2",
            "2ef3cd54-df28-40d4-bc61-08d804c488ea",
            "cb1d2807-d91e-41cc-de3f-08d817a04886",
            "51b288c1-d4a7-4371-33eb-08d81dc47c96",
            "bf3e4d12-4932-43a9-34c1-08d888d6cd8e",
            "4085f370-08d8-4704-34c3-08d888d6cd8e",
            "5ce260d3-509b-4f20-bf4d-4c679c109971",
            "2c409cc0-3ee6-4403-bd66-9913e5174752",
            "1ba9eeae-77ab-48d2-abb1-c758c4780d5e",
            "f4a52c81-9510-4d5e-bd9d-e6d67698d9da",
            "6e493130-627f-4d18-f46d-08d75d796e77",
            "f4e45fc6-a11e-4e8d-6e05-08d7616ac1d6",
            "86ae276c-0cbf-4010-0eaf-08d76170dd2e",
            "0ab3a091-b901-4642-08b4-08d76837cc2a",
            "1ba77710-bb83-43b4-eb7c-08d76c81d293",
            "c7fcaec4-a641-41bd-b262-08d77f33495b",
            "4b59dcf5-ed95-4813-ce7b-08d79b0e2c11",
            "595560ae-1b95-4b5a-5e51-08d7c09a34f6",
            "aa43e4e9-41ef-46ed-e0d8-08d7e243eba7",
            "6e096fe8-4fd7-4c0b-e0da-08d7e243eba7",
            "a8219d4f-3edb-45d3-6aa8-08d7fe78dcf7",
            "2332bfaa-bc65-4f6a-9629-08d828052de9",
            "74465dd5-ddf9-42f3-a6b0-08d871d90171",
            "71d59135-d5c2-42ab-a6d8-08d871d90171",
            "aa9c6c9a-5138-4f33-5087-08d879ea80c5",
            "d915e2a3-e383-4119-35f4-08d87e25db67",
            "1019b526-db1c-4d00-35fe-08d87e25db67",
            "28d3f10f-58ac-4a6a-34a9-08d888d6cd8e",
            "60965e55-99ff-4f06-34af-08d888d6cd8e",
            "f2703ccf-107f-4e87-34b3-08d888d6cd8e",
            "3bebd87f-1356-4405-34b7-08d888d6cd8e",
            "8f5b1cac-1406-4b81-34bf-08d888d6cd8e",
            "6ce4a4a7-ab64-4128-a3f3-08d6ffb1f98d",
            "03c4550f-2210-4584-3a38-08d7088bd51f",
            "2b1df939-49c7-407e-0022-08d70a8f819f",
            "e2cb100d-eb71-4fbb-3f0a-08d73df91f82",
            "02b0df37-bbe3-4d69-0511-08d7777faa61",
            "a3b3c394-31f9-4af6-190b-08d782416dca",
            "40e542dc-3d75-422a-c2cb-08d79f9d66f7",
            "8717db5f-f6bd-406c-c2cc-08d79f9d66f7",
            "2a5d66e3-bef2-4f34-48e4-08d7a5d637b1",
            "7f933b0a-c910-4ce4-48f6-08d7a5d637b1",
            "94cf4369-f3c8-40b5-48f7-08d7a5d637b1",
            "6f50d224-b4dc-4ce7-afcf-08d7b5855a54",
            "c3e3727d-a71f-403c-6642-08d7befdda40",
            "0a97013d-d1ca-4fb0-351b-08d7cf48e0ee",
            "fe315cd9-c00c-4e80-33ed-08d81dc47c96",
            "b4f8dda6-90f0-45c2-33ef-08d81dc47c96",
            "127bed14-1a72-483e-41d2-08d821b19c64",
            "6e2a315b-6ba6-437e-41d3-08d821b19c64",
            "66a8fa72-ea9f-428f-e95f-08d82c8a6ae7",
            "06f9e07c-d519-42d7-b573-08d82ffce8bc",
            "49222fc7-7d19-4c8e-b3cd-08d843d0a20b",
            "dccd93a1-56ff-44a0-7942-08d84c69c483",
            "8613d6ae-940e-4883-795a-08d84c69c483",
            "18e45175-ba01-4d20-aba2-08d86ae3c5dd",
            "61394db0-78b0-4077-aba5-08d86ae3c5dd",
            "d64915d8-fb08-49f3-5086-08d879ea80c5",
            "16a1ff69-8ef8-438d-35fd-08d87e25db67",
            "70668f1d-b8b2-453c-34b0-08d888d6cd8e",
            "a3f4083d-f1e1-43a3-34c0-08d888d6cd8e",
            "3fbc5878-fd31-4084-1879-08d89c1bf4f3",
            "bc7f6bf7-98a6-4ce3-a659-08d8b37403c1",
            "fc290d66-2d31-403d-a74c-08d8c96178ee",
            "2c581216-4b69-4838-290f-08d8cc6ecf60",
            "24478b83-c7eb-41c7-2efe-08d8d044acc9",
            "8ed37da0-1453-4e7b-d769-08d8dc427db2",
            "6b9a2883-2add-44b9-d7cd-08d8dc427db2",
            "6fcf76b4-c1bb-4c0e-0482-08d6d3fec2a0",
            "abec0aa1-baf5-4454-3e59-08d73df91f82",
            "44dac0e6-a3bc-4d77-6719-08d8daca3168"
        };

        private readonly MixyBoosContext _context;
        private readonly ILogger _logger;
        private readonly Faker _faker;
        private readonly Random _rand;

        public TestData(MixyBoosContext context, ILogger logger) {
            _context = context;
            _logger = logger;
            _faker = new Faker("en");
            _rand = new Random();
        }

        public async Task<List<MixyBoosUser>> GetTestUsers() {
            var users = new List<MixyBoosUser>() {
                new() {
                    DisplayName = "Fergal Moran",
                    Email = "fergal.moran+mixyboos@gmail.com",
                    NormalizedEmail = "FERGAL.MORAN+MIXYBOOS@GMAIL.COM",
                    UserName = "fergal.moran+mixyboos@gmail.com",
                    NormalizedUserName = "FERGAL.MORAN+MIXYBOOS@GMAIL.COM",
                    PhoneNumber = "+111111111111",
                    StreamKey = "YfbUdfzcgjgIXvUaNZ3X9lQoyhdEc6nc",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString("D"),
                    ProfileImage = _faker.Internet.Avatar(),
                    HeaderImage = _faker.Image.LoremFlickrUrl(),
                    Title = _faker.Name.JobTitle()
                },
                new() {
                    DisplayName = "I am follower",
                    Email = "fergal.moran+mixyboosfollower@gmail.com",
                    NormalizedEmail = "FERGAL.MORAN+MIXYBOOSFOLLOWER@GMAIL.COM",
                    UserName = "fergal.moran+mixyboosfollower@gmail.com",
                    NormalizedUserName = "FERGAL.MORAN+MIXYBOOSFOLLOWER@GMAIL.COM",
                    PhoneNumber = "+111111111111",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString("D"),
                    ProfileImage = _faker.Internet.Avatar(),
                    HeaderImage = _faker.Image.LoremFlickrUrl(),
                    Title = _faker.Name.JobTitle()
                },
            };
            return await Task.FromResult(users);
        }

        public async Task<List<LiveShow>> GetTestShows() {
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.UserName.Equals("fergal.moran+mixyboos@gmail.com"));

            if (user == null) {
                throw new Exception("Unable to find seed user");
            }

            return new List<LiveShow> {
                new() {
                    Title = "Test Show One",
                    StartDate = DateTime.UtcNow,
                    IsFinished = true,
                    User = user
                }
            };
        }

        public async Task<List<Mix>> GetTestMixes() {
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.UserName.Equals("fergal.moran+mixyboos@gmail.com"));

            if (user == null) {
                throw new Exception("Unable to find seed user");
            }

            short mixIndex = 0;
            var results = new List<Mix>();
            for (int i = 0; i < MAX_MIX_COUNT; i++) {
                var mixUrl = $"https://cdn.podnoms.com/audio/${MIXES[mixIndex]}.mp3";
                var image = _faker.Image.LoremFlickrUrl();
                var m = new Mix {
                    Title = _faker.Lorem.Sentence(),
                    Description = _faker.Lorem.Paragraphs(_rand.Next(1, 5)),
                    Image = image,
                    User = user,
                    AudioUrl = mixUrl,
                };

                if (mixIndex == MIXES.Length - 1) {
                    mixIndex = 0;
                } else {
                    mixIndex++;
                }

                _logger.LogDebug($"Scaffolding {m.Title} - {i} of {MAX_MIX_COUNT}");
                results.Add(m);
            }

            return await Task.FromResult(results);
        }
    }
}
