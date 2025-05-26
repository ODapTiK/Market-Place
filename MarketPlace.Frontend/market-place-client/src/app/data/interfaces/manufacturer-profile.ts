import { Notification } from "./notification";

export interface ManufacturerProfile {
    id: string;
    organization: string;
    logo: string | null;
    manufacturerNotifications: Notification[];
  }
