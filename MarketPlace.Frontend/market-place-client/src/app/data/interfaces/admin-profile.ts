export interface AdminProfile {
    id: string;
    name: string;
    surname: string;
    logo?: string | null;
    adminControlOrdersId: string[];
}
