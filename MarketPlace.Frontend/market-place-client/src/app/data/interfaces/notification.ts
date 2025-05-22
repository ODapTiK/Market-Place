export interface Notification {
    id: string;
    title: string;
    message: string;
    type: string;
    createdAt: Date;
    isRead: boolean;
}
