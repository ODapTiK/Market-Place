import { Notification } from "./notification";

export interface AdminProfile {
    id: string;
    name: string;
    surname: string;
    logo?: string | null;
    adminControlOrdersId: string[];
    adminNotifications: Notification[];
}
