import { Notification } from "./notification";

export interface UserProfile {
    id: string;
    name: string;
    surname: string;
    birthDate: string;
    logo?: string | null;
    userOrdersId: string[];
    userNotifications: Notification[];
}
