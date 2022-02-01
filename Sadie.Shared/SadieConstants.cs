namespace Sadie.Shared;

public static class SadieConstants
{
    public static int HabboPacketMinLength => 4;
    public static int HabboPacketMaxLength => 4000;
    public static int HabboSsoMinLength => 20;

    public static string HabboPolicyXml => "<?xml version=\"1.0\"?>\r\n" +
                                           "<!DOCTYPE cross-domain-policy SYSTEM \"/xml/dtds/cross-domain-policy.dtd\">\r\n" +
                                           "<cross-domain-policy>\r\n" +
                                           "<allow-access-from domain=\"*\" to-ports=\"1-31111\" />\r\n" +
                                           "</cross-domain-policy>\x0";

    public static string DateTimeFormat => "yyyy-MM-dd HH:mm:ss";
    
    public static string MockHeightmap = "00000000\r\n00000000\r\n00000000\r\n00000000\r\n00000000\r\n00000000\r\n00000000\r\n00000000";
}