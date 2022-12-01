export class MediaInfoDto {
    id!: string;
    name!: string;
    theme!: string;
    fullName!: string;
    creationTime!: Date;
    previewImage!: string;

    constructor(init?: Partial<MediaInfoDto>) {
      Object.assign(this, init);
    }
}
