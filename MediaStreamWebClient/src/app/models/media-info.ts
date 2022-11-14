export class MediaInfoDto {
    name!: string;
    fullName!: string;
    creationTime!: Date;
    previewImage!: string;

    constructor(init?: Partial<MediaInfoDto>) {
        Object.assign(this, init)
    }
}
