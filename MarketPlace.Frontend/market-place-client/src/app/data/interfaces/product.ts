import { Review } from "./review";

export interface Product {
    id: string;
    manufacturerId: string;
    name: string | null;
    description: string | null;
    category: string | null;
    type: string | null;
    reviews: Review[];
    image: string | null;
    price: number;
    raiting: number;
    viewAt: string[];
    creationDateTime: string;
  }
