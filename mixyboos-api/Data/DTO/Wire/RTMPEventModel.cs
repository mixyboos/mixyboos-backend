namespace MixyBoos.Api.Data.DTO.Wire;

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class Identifier {
  public Rtmp Rtmp { get; set; }
}

public class Info {
  public string id { get; set; }
  public string sub_type { get; set; }
  public NotifyInfo notify_info { get; set; }
}

public class NotifyInfo {
  public string request_url { get; set; }
  public string remote_addr { get; set; }
}

public class Publish {
  public Identifier identifier { get; set; }
  public Info info { get; set; }
}

public class RTMPEventModel {
  public Publish Publish { get; set; }
}

public class Rtmp {
  public string app_name { get; set; }
  public string stream_name { get; set; }
}
